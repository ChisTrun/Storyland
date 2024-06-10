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

            const response = await fetch(`${BE_HOST}/api/search/${storyServer}/tacgia/${encodeURIComponent(authorId)}?page=${curPage}&limit=${perPage}`);
            let resBody = {};
            if (!response.ok) {
                resBody.data = [];
                const errorMessage = await response.text();
                console.error(`${response.status}: ${errorMessage}`);
            }
            else {
                resBody = await response.json();
            }
            const serverArr = await getServerArr();
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
            next(new ErrorDisplay("Tìm kiếm tác giả thất bại!", 500, error.message));
        }
    },
};