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
        let history = req.session.history;
        
        const entries = Object.entries(history);
        const filteredEntries = entries.filter(([key, value]) => {
            const storyServer = (key.split('-server-'))[1];
            return sortedServerIds.includes(storyServer);
        });
        history  = Object.fromEntries(filteredEntries);
        req.session.history = history;
        
        render.stories = history;
        render.sortedServerIds = sortedServerIds;
        render.isDark = req.session.isDark;
        render.title = `Lịch sử | StoryLand`;

        return res.render(view, render, null);
    },
    delete(req, res, next) {
        const storyId = decodeURIComponent(req.body.storyId);
        const storyServer = req.body.storyServer;
        delete req.session.history[`${storyId}-server-${storyServer}`];
        res.end();
    },
};