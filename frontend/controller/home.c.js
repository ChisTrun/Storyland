const view = 'home';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
};

module.exports = {
    async render(req, res, next) {
        return res.render(view, render, null);
    },
};