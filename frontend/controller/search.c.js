const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr } = require('../utils/utils');

const view = 'search';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};
const perPage = 24;

module.exports = {
    async render(req, res, next) {
        try {
            const sortedServerIds = req.session.sortedServerIds;
            const keyword = req.query.keyword || ' ';
            const curPage = parseInt(req.query.page) || 1;
            
            const url = `${BE_HOST}/api/search/${sortedServerIds[0]}/truyen/${encodeURIComponent(keyword)}?page=${curPage}&limit=${perPage}`;
            const [responseResult, serverArrResult] = await Promise.allSettled([
                fetch(url),
                getServerArr()
            ]);
            let resBody = {};
            if (responseResult.status === 'fulfilled' && responseResult.value.ok) {
                resBody = await responseResult.value.json();
            }
            else if (responseResult.status === 'fulfilled' && !responseResult.value.ok) {
                resBody.data = [];
                const errorMessage = await responseResult.value.text();
                console.error(`Error fetching api to get stories of the keyword ${keyword}: ${responseResult.value.status} - ${errorMessage}`);
            }
            else {
                resBody.data = [];
                console.error(`Error fetching request: ${responseResult.hasOwnProperty("reason") ? responseResult.reason : "pending"}`);
            }
            const serverArr = serverArrResult.status === 'fulfilled' ? serverArrResult.value : [];
            const totalPages = resBody.totalPages && resBody.totalPages > 0 ? resBody.totalPages : 1;

            render.stories = resBody.data;
            render.keyword = keyword;
            render.curPage = curPage;
            render.totalPages = totalPages;

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