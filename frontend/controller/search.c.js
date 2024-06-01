const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

const view = 'search';
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
            const keyword = req.query.keyword || ' ';
            const curPage = parseInt(req.query.page) || 1;
            
            const response = await fetch(`${BE_HOST}/api/search/${serverIndex}/truyen/${encodeURIComponent(keyword)}?page=${curPage}&limit=${perPage}`);
            const resBody = await response.json();
            const totalPages = resBody.totalPages ? resBody.totalPages : 1;
            if (curPage > totalPages) {
                res.redirect('back');
            }

            render.stories = resBody.data;
            render.keyword = keyword;
            render.curPage = curPage;
            render.totalPages = totalPages;

            render.serverIndex = serverIndex;
            render.isDark = req.session.isDark;
            render.title = "Kết quả tìm kiếm | StoryLand";

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm thất bại", 503, error.message));
        }
    },
};