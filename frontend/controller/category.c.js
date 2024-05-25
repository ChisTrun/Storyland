const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

const view = 'category';
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
            const curServer = req.params.serverIndex;
            const categoryId = req.query.id;
            const categoryName = req.params.categoryName;
            const curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/category/${curServer}/${categoryId}?page=${curPage}&limit=${perPage}`);
            const resBody = await response.json();
            const totalPages = resBody.totalPages ? resBody.totalPages : 1;

            render.curServer = curServer;
            render.stories = resBody.data;
            render.categoryName = categoryName;
            render.categoryId = categoryId;
            render.curPage = curPage;
            render.totalPages = totalPages;

            render.serverIndex = req.session.serverIndex;
            render.isDark = req.session.isDark;
            render.title = `Thể loại ${categoryName} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm thể loại thất bại", 503, error.message));
        }
    },
    async getAll(req, res, next) {
        try {
            const response = await fetch(`${BE_HOST}/api/category/${req.params.serverIndex}`);
            const resBody = await response.json();

            return res.json(resBody);
        }
        catch (error) {
            console.log(error.message);
            return res.json({});
        }
    },
};