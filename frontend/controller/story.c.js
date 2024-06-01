const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

const view = 'story';
const render = {
    layout: 'main',
    scripts: ['/js/story.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        try {
            const storyServer = req.params.storyServer;
            const storyId = decodeURIComponent(req.params.storyId);
            const serverIndex = req.session.serverIndex;

            const storyResponse = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}`);
            const storyResBody = await storyResponse.json();
            const serverResponse = await fetch(`${BE_HOST}/api/server`);
            const serverResBody = await serverResponse.json();

            let desc = storyResBody.description.replace(/\r\n\r\n/g, '<br>')
                .replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
            storyResBody.description = desc.replace(/<br>(\s*&nbsp;)*/g, '<br>');
            Object.assign(render, {
                ...storyResBody
            });
            render.storyServer = storyServer;
            render.storyId = storyId;
            render.storyFirstIndex = 0;
            render.serverArr = serverResBody;

            if (req.session.history[`${storyId}-server-${storyServer}`]) {
                const story = req.session.history[`${storyId}-server-${storyServer}`];
                render.storyConIndex = story.chapterIndex;
            }

            render.serverIndex = serverIndex;
            render.isDark = req.session.isDark;
            render.title = `${storyResBody.name} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem thông tin truyện thất bại", 503, error.message));
        }
    },
};