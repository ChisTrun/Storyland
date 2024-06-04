const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr } = require('../utils/utils');

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

            const response = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();
            const serverArr = await getServerArr();

            let desc = resBody.description.replace(/\r\n\r\n/g, '<br>')
                .replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
            resBody.description = desc.replace(/<br>(\s*&nbsp;)*/g, '<br>');
            Object.assign(render, {
                ...resBody
            });
            render.storyServer = storyServer;
            render.storyId = storyId;
            render.storyFirstIndex = 0;
            render.serverArr = serverArr;

            if (req.session.history[`${storyId}-server-${storyServer}`]) {
                const story = req.session.history[`${storyId}-server-${storyServer}`];
                render.storyConIndex = story.chapterIndex;
            }

            render.serverIndex = serverIndex;
            render.isDark = req.session.isDark;
            render.title = `${resBody.name} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem thông tin truyện thất bại!", 500, error.message));
        }
    },
};