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
            const serverIndex = req.session.serverIndex;
            const categoryId = decodeURIComponent(req.query.id);
            const categoryName = req.params.categoryName;
            const curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/category/${serverIndex}/${encodeURIComponent(categoryId)}?page=${curPage}&limit=${perPage}`);
            const resBody = await response.json();
            const totalPages = resBody.totalPages ? resBody.totalPages : 1;
            if (curPage > totalPages) {
                return res.redirect('back');
            }
            
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
            const serverIndex = req.session.serverIndex;
            const response = await fetch(`${BE_HOST}/api/category/${serverIndex}`);
            const resBody = await response.json();

            return res.json(resBody);
        }
        catch (error) {
            console.log(error.message);
            return res.json([]);
        }
    },
};