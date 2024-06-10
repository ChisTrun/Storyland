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
            const sortedServerIds = req.session.sortedServerIds;
            const categoryName = req.params.categoryName;
            const categoryId = decodeURIComponent(req.query.id);
            const curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/category/${sortedServerIds[0]}/${encodeURIComponent(categoryId)}?page=${curPage}&limit=${perPage}`);
            let resBody = {};
            if (!response.ok) {
                resBody.data = [];
                const errorMessage = await response.text();
                console.error(`${response.status}: ${errorMessage}`);
            }
            else {
                resBody = await response.json();
            }
            const serverArr = await getServerArr();
            const totalPages = resBody.totalPages && resBody.totalPages > 0 ? resBody.totalPages : 1;

            render.stories = resBody.data;
            render.categoryName = categoryName;
            render.categoryId = categoryId;
            render.curPage = curPage;
            render.totalPages = totalPages;

            render.curServer = serverArr.find(server => server.id === sortedServerIds[0]);
            render.sortedServerIds = sortedServerIds;
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
            const sortedServerIds = req.session.sortedServerIds;
            const response = await fetch(`${BE_HOST}/api/category/${sortedServerIds[0]}`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();
            res.json(resBody);
        }
        catch (error) {
            console.error(error.message);
            res.json([]);
        }
    },
};