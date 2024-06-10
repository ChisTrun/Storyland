const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr } = require('../utils/utils');

const view = 'author';
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
            const authorName = req.params.authorName;
            const authorServer = req.params.authorServer;
            const authorId = decodeURIComponent(req.query.id);
            const curPage = parseInt(req.query.page) || 1;

            const url = `${BE_HOST}/api/search/${authorServer}/tacgia/${encodeURIComponent(authorId)}?page=${curPage}&limit=${perPage}`;
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
                console.error(`Error fetching api to get stories of the author ${authorId}: ${responseResult.value.status} - ${errorMessage}`);
            }
            else {
                resBody.data = [];
                console.error(`Error fetching request: ${responseResult.hasOwnProperty("reason") ? responseResult.reason : "pending"}`);
            }
            const serverArr = serverArrResult.status === 'fulfilled' ? serverArrResult.value : [];
            const totalPages = resBody.totalPages && resBody.totalPages > 0 ? resBody.totalPages : 1;

            render.stories = resBody.data;
            render.authorName = authorName;
            render.authorId = authorId;
            render.curPage = curPage;
            render.totalPages = totalPages;

            render.curServer = serverArr.find(server => server.id === authorServer);
            render.sortedServerIds = sortedServerIds;
            render.isDark = req.session.isDark;
            render.title = `Tác giả ${render.authorName} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm tác giả thất bại!", 503, error.message));
        }
    },
};