const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr } = require('../utils/utils');

const view = 'search';
const render = {
    layout: 'main',
    scripts: ['/js/search.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};
const perPage = 24;

module.exports = {
    async render(req, res, next) {
        try {
            const minChapter = req.query.minChapter || 0;
            const maxChapter = req.query.maxChapter || 10000;
            const sortedServerIds = req.session.sortedServerIds;
            const keyword = req.query.keyword || ' ';
            let curPage = parseInt(req.query.page) || 1;
            
            const url = `${BE_HOST}/api/search/all/truyen/${encodeURIComponent(keyword)}/${minChapter === 0 ? -1 : minChapter}/${maxChapter === 10000 ? -1 : maxChapter}/all`;        
            const [responseResult, serverArrResult] = await Promise.allSettled([
                fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(sortedServerIds)
                }),
                getServerArr()
            ]);
            let resBody = [];
            if (responseResult.status === 'fulfilled' && responseResult.value.ok) {
                resBody = await responseResult.value.json();
            }
            else if (responseResult.status === 'fulfilled' && !responseResult.value.ok) {
                resBody = [];
                const errorMessage = await responseResult.value.text();
                console.error(`Error fetching api to get stories of the keyword ${keyword}: ${responseResult.value.status} - ${errorMessage}`);
            }
            else {
                resBody = [];
                console.error(`Error fetching request: ${responseResult.hasOwnProperty("reason") ? responseResult.reason : "pending"}`);
            }
            const serverArr = serverArrResult.status === 'fulfilled' ? serverArrResult.value : [];
            
            const filteredStories = resBody.filter(story => story.numberOfChapter >= minChapter && story.numberOfChapter <= maxChapter);

            const totalStories = filteredStories.length;
            const totalPages = totalStories !== 0 ? Math.ceil(totalStories / perPage) : 1;
            curPage = curPage > totalPages ? totalPages : curPage;
            const startIndex = (curPage - 1) * perPage;
            const endIndex = startIndex + perPage;
            const paginatedStories = filteredStories.slice(startIndex, endIndex);

            render.stories = paginatedStories;
            render.keyword = keyword;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.minChapter = minChapter;
            render.maxChapter = maxChapter;

            render.curServer = serverArr.find(server => server.id === sortedServerIds[0]);
            render.sortedServerIds = sortedServerIds;
            render.isDark = req.session.isDark;
            render.title = "Kết quả tìm kiếm | StoryLand";

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm từ khoá thất bại!", 503, error.message));
        }
    },
};