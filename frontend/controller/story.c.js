const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr, getNumChaptersInStory } = require('../utils/utils');

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
            const sortedServerIds = req.session.sortedServerIds;
            const storyServer = req.params.storyServer;
            const storyId = decodeURIComponent(req.params.storyId);

            const results = await Promise.allSettled([
                fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}`),
                getServerArr(),
                getNumChaptersInStory(storyId, sortedServerIds),
            ]);
            const [responseResult, serverArrResult, chapterCountResult] = results;
            // responseResult
            if (responseResult.status !== 'fulfilled') {
                throw Error(`Error fetching request: ${responseResult.hasOwnProperty("reason") ? responseResult.reason : "pending"}`);
            }
            const response = responseResult.value;
            if (!response.ok) {
                const errorMessage = await response.text();
                next(new ErrorDisplay("Xem thông tin truyện thất bại!", response.status, errorMessage));
            }
            const resBody = await response.json();
            // serverArrResult
            const serverArr = serverArrResult.status === 'fulfilled' ? serverArrResult.value : [];
            // chapterCountResult
            const chapterCount = chapterCountResult.status === 'fulfilled' ? chapterCountResult.value : 0;

            let desc = resBody.description.replace(/\r\n\r\n/g, '<br>')
                .replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
            resBody.description = desc.replace(/<br>(\s*&nbsp;)*/g, '<br>');

            Object.assign(render, {
                ...resBody
            });
            render.storyId = storyId;
            render.storyFirstIndex = (chapterCount > 0 ? 0 : null);
            if (req.session.history[`${storyId}-server-${storyServer}`]) {
                render.storyConIndex = (req.session.history[`${storyId}-server-${storyServer}`]).chapterIndex;
            }

            render.chapterCount = chapterCount;
            render.curServer = serverArr.find(server => server.id === storyServer);
            render.sortedServerIds = sortedServerIds;
            render.isDark = req.session.isDark;
            render.title = `${resBody.name} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem thông tin truyện thất bại!", 503, error.message));
        }
    }
};