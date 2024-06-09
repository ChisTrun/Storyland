const view = 'offlinereading';
const { HOST, PORT } = require('../global/env');

const render = {
    layout: 'main',
    scripts: ['js/offlinereading.js'],
    styles: [],
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        render.serverIndex = req.session.serverIndex;
        render.isDark = req.session.isDark;
        render.title = "Trình đọc truyện | StoryLand";
        
        return res.render(view, render, null);
    },
};