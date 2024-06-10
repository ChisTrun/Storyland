const { HOST, PORT } = require('../global/env');

const view = 'offlinereading';
const render = {
    layout: 'main',
    scripts: ['/js/jszip.js', '/js/epub.js', 'js/offlinereading.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        render.sortedServerIds = req.session.sortedServerIds;
        render.isDark = req.session.isDark;
        render.title = "Trình đọc truyện đã tải | StoryLand";

        return res.render(view, render, null);
    },
};