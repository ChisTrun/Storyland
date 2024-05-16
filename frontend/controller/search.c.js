const view = 'search';
const render = {
    layout: 'main',
    scripts: ['/setting.client.js', '/page/home.client.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
};
const { BE_HOST } = require('../global/env');
const perPage = 36;

module.exports = {
    async render(req, res, next) {
        try {
            const keyword = req.query.keyword || '';
            let curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/search/truyen/${keyword}`);
            const data = await response.json();

            const totalPages = Math.ceil(data.length / perPage);
            curPage = Math.min(Math.max(parseInt(curPage), 1), totalPages);

            render.stories = data.slice((curPage - 1) * perPage, curPage * perPage);
            render.keyword = keyword;
            render.curPage = curPage;
            render.totalPages = totalPages;

            return res.render(view, render, null);
        }
        catch (error) {
            console.error(error);
            next(error);
        }
    },
};
