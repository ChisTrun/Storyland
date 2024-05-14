const view = 'search';
const render = {
    layout: 'main',
    scripts: ['/setting.client.js', '/page/home.client.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
};
const Story = require('../model/story.m')

module.exports = {
    async render(req, res, next) {
        return res.render(view, render, null);
        try 
        {
            chaptersList = await Chapter.getAll(req.params.storyName);
            return res.send(chaptersList);
        }
        catch (error) 
        {
            console.error(error);
            return res.send(error);
        }
    },
};
