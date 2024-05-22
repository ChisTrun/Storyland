const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST,HOST,PORT } = require('../global/env');
const view = 'author';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host : `https://${HOST}:${PORT}`,
};
const perPage = 24;
const serverIndex = 0;

module.exports = {
    async render(req, res, next) {
        try {
            const authorId = encodeURIComponent(`?author=${req.query.author}`);
            const authorName = req.params.authorName;
            const curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/search/${serverIndex}/tacgia/${authorId}?page=${curPage}&limit=${perPage}`);
            const resBody = await response.json();
            const totalPages = resBody.totalPages ?  resBody.totalPages : 1;

            render.serverIndex = req.session.serverIndex
            render.stories = resBody.data;
            render.authorName = authorName;
            render.authorId = req.query.author;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.title = `Tác giả ${authorName}`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm tác giả thất bại", 503, error.message));
        }
    },
};
