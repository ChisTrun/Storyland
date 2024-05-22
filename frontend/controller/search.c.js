const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST,HOST,PORT } = require('../global/env');
const view = 'search';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host : `https://${HOST}:${PORT}`,
};
const perPage = 24;


module.exports = {
    async render(req, res, next) {
        try {
            const keyword = req.query.keyword || '';
            const curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/search/${req.session.serverIndex}/truyen/${keyword}?page=${curPage}&limit=${perPage}`);
            const resBody = await response.json();
            const totalPages = resBody.totalPages ?  resBody.totalPages : 1;

            render.serverIndex = req.session.serverIndex
            render.stories = resBody.data;
            render.keyword = keyword;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.title = "Kết quả tìm kiếm";

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm thất bại", 503, error.message));
        }
    },
};
