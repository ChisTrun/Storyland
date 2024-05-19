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

module.exports = {
    async render(req, res, next) {
        try {
            const categoryId = req.query.ctg;
            const path = encodeURIComponent(`?ctg=${categoryId}`);
            const categoryName = req.params.categoryName;
            let curPage = parseInt(req.query.page) || 1;

            console.log(`${BE_HOST}/api/category/${path}/all`);
            const data = await response.json();

            const totalPages = data.length == 0 ? 1 : Math.ceil(data.length / perPage);
            curPage = Math.min(Math.max(parseInt(curPage), 1), totalPages);

            render.stories = data.slice((curPage - 1) * perPage, curPage * perPage);
            render.categoryName = categoryName;
            render.categoryId = categoryId;
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
