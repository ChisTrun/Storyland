const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr } = require('../utils/utils');

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

            let resBody = {};
            const response = await fetch(`${BE_HOST}/api/category/${serverIndex}/${encodeURIComponent(categoryId)}?page=${curPage}&limit=${perPage}`);
            if (!response.ok) {
                resBody.data = [];
                resBody.totalPages = 0;
                const errorMessage = await response.text();
                console.error(errorMessage)
            }
            else {
                resBody = await response.json();
            }
            const serverArr = await getServerArr();
            const totalPages = resBody.totalPages ? resBody.totalPages : 1;
            curPage <= totalPages || res.redirect('back');
            
            render.stories = resBody.data;
            render.categoryName = categoryName;
            render.categoryId = categoryId;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.serverArr = serverArr;

            render.serverIndex = req.session.serverIndex;
            render.isDark = req.session.isDark;
            render.title = `Thể loại ${categoryName} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm thể loại thất bại!", 500, error.message));
        }
    },
    async getAll(req, res, next) {
        try {
            const serverIndex = req.session.serverIndex;
            const response = await fetch(`${BE_HOST}/api/category/${serverIndex}`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();

            return res.json(resBody);
        }
        catch (error) {
            console.error(error.message);
            return res.json([]);
        }
    },
};