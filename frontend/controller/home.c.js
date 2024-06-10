const { HOST, PORT } = require('../global/env');

const view = 'home';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        const errorMessage = req.session.errorMessage;
        req.session.errorMessage = null;

        render.errorMessage = errorMessage;
        render.sortedServerIds = req.session.sortedServerIds;
        render.isDark = req.session.isDark;
        render.title = "Trang chủ | StoryLand";
        
        return res.render(view, render, null);
    },
};