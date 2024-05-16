const view = 'story';
const render = {
    layout: 'main',
    scripts: ['/setting.client.js', '/page/home.client.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
};
const { BE_HOST } = require('../global/env');
const perPage = 50;

module.exports = {
    async render(req, res, next) {
        try 
        {
            storyName = req.params.storyName;
            let curPage = parseInt(req.query.page) || 1;

            const response = await fetch(`${BE_HOST}/api/story/${storyName}`);
            const data = await response.json();

            const totalPages = Math.ceil(data.length / perPage);
            curPage = Math.min(Math.max(parseInt(curPage), 1), totalPages);

            render.storyName = storyName;
            render.chapters = data.slice((curPage - 1) * perPage, curPage * perPage);
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.totalChapters = data ? data.length : 0;
            render.latestIndex = data ? data[data.length-1].index : 0;

            return res.render(view, render, null);
        }
        catch (error) 
        {
            console.error(error);
            next(error);
        }
    },
};
