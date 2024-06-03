const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

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
            const serverIndex = req.session.serverIndex;
            const authorId = decodeURIComponent(req.query.id);
            const authorName = req.params.authorName;
            const curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/search/${serverIndex}/tacgia/${encodeURIComponent(authorId)}?page=${curPage}&limit=${perPage}`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();
            const totalPages = resBody.totalPages ? resBody.totalPages : 1;
            curPage <= totalPages || res.redirect('back');
            

            render.stories = resBody.data;
            render.authorName = authorName;
            render.authorId = authorId;
            render.curPage = curPage;
            render.totalPages = totalPages;

            render.serverIndex = req.session.serverIndex;
            render.isDark = req.session.isDark;
            render.title = `Tác giả ${render.authorName} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm tác giả thất bại!", 500, error.message));
        }
    },
};