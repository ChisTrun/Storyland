const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const view = 'history';
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
    render(req, res, next) {
        // const curPage = parseInt(req.query.page) || 1;
        const data = req.session.history;
        // const dataLength = Object.keys(data).length;
        // const totalPages = Math.ceil(dataLength / perPage);

        // render.stories = data.slice((curPage - 1) * perPage, curPage * perPage);
        render.stories = req.session.history;
        // render.curPage = curPage;
        // render.totalPages = totalPages;
        render.title = `Lịch sử`;
        render.isDark = req.session.isDark;
        return res.render(view, render, null);
    },
    delete(req, res, next) {
        return 0;
    }
};
