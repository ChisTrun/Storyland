const view = 'home';
const render = {
    layout: 'main',
    scripts: ['/setting.client.js', '/page/home.client.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
};


module.exports = {
    async render(req, res, next) {
        return res.render(view, render, null);
    },
};
