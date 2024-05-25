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
        const data = req.session.history;

        render.stories = data;

        render.serverIndex = req.session.serverIndex;
        render.isDark = req.session.isDark;
        render.title = `Lịch sử | StoryLand`;

        return res.render(view, render, null);
    },
    delete(req, res, next) {
        const storyId = decodeURIComponent(req.body.storyId);
        const server = req.body.server;
        delete req.session.history[`${storyId}-server-${server}`];
        return res.json({success: true});
    },
};