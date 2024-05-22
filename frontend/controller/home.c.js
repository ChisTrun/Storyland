const view = 'home';
const { HOST,PORT } = require('../global/env');

const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host : `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        render.serverIndex = req.session.serverIndex
        return res.render(view, render, null);
    },
};