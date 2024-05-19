const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');
const view = 'author';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
};
const perPage = 24;

module.exports = {
    async render(req, res, next) {
        try {
            const authorId = req.query.author;
            const path = encodeURIComponent(`?author=${authorId}`);
            const authorName = req.params.authorName;
            let curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/search/tacgia/${path}/all`);
            const data = await response.json();

            const totalPages = data.length == 0 ? 1 : Math.ceil(data.length / perPage);
            curPage = Math.min(Math.max(parseInt(curPage), 1), totalPages);

            render.stories = data.slice((curPage - 1) * perPage, curPage * perPage);
            render.authorName = authorName;
            render.authorId = authorId;
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
