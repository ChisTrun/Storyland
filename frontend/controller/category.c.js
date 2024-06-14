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
            const sortedServerIds = req.session.sortedServerIds;
            const categoryName = req.params.categoryName;
            const categoryId = decodeURIComponent(req.query.id);
            let curPage = parseInt(req.query.page) || 1;

            const url = `${BE_HOST}/api/search/all/${encodeURIComponent(categoryId)}/all`;
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(sortedServerIds)
            });
            let resBody = [];
            if (response.ok) {
                resBody = await response.json();
            }
            else {
                resBody = [];
                const errorMessage = await response.text();
                console.error(`Error fetching api to get stories of the category ${categoryId}: ${response.status} - ${errorMessage}`);
            }
            const totalStories = resBody.length;
            const totalPages = totalStories !== 0 ? Math.ceil(totalStories / perPage) : 1;
            curPage = curPage > totalPages ? totalPages : curPage;
            const startIndex = (curPage - 1) * perPage;
            const endIndex = startIndex + perPage;
            const paginatedStories = resBody.slice(startIndex, endIndex);

            render.stories = paginatedStories;
            render.categoryName = categoryName;
            render.categoryId = categoryId;
            render.curPage = curPage;
            render.totalPages = totalPages;

            render.sortedServerIds = sortedServerIds;
            render.isDark = req.session.isDark;
            render.title = `Thể loại ${categoryName} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Tìm kiếm thể loại thất bại!", 503, error.message));
        }
    },
    async getAll(req, res, next) {
        try {
            const sortedServerIds = req.session.sortedServerIds;
            const response = await fetch(`${BE_HOST}/api/category/all`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(sortedServerIds)
            });
            if (!response.ok) {
                throw Error();
            }
            const resBody = await response.json();
            res.json(resBody);
        }
        catch (error) {
            res.json([]);
        }
    },
};