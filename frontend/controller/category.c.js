const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');
const view = 'category';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
};
const perPage = 24;
const serverIndex = 0;

module.exports = {
    async render(req, res, next) {
        try {
            const categoryId = encodeURIComponent(`?ctg=${req.query.ctg}`);
            const categoryName = req.params.categoryName;
            const curPage = parseInt(req.query.page) || 1;
            
            const response = await fetch(`${BE_HOST}/api/category/${serverIndex}/${categoryId}?page=${curPage}&limit=${perPage}`);
            const resBody = await response.json();
            const totalPages = resBody.totalPages ?  resBody.totalPages : 1;

            render.stories = resBody.data;
            render.categoryName = categoryName;
            render.categoryId = req.query.ctg;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.title = `Thể loại ${categoryName}`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm thể loại thất bại", 503, error.message));
        }
    },
};
