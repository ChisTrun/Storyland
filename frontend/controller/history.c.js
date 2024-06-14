const { HOST, PORT } = require('../global/env');

const view = 'history';
const render = {
    layout: 'main',
    scripts: ['/js/history.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    render(req, res, next) {
        const sortedServerIds = req.session.sortedServerIds;
        const history = req.session.history;

        render.stories = history;
        render.sortedServerIds = sortedServerIds;
        render.isDark = req.session.isDark;
        render.title = `Lịch sử | StoryLand`;

        return res.render(view, render, null);
    },
    delete(req, res, next) {
        const storyId = decodeURIComponent(req.body.storyId);
        delete req.session.history[storyId];
        res.end();
    },
};