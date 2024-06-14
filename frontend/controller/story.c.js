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
            const storyId = decodeURIComponent(req.params.storyId);

            const results = await Promise.allSettled([
                getServerArr(),
                getNumChaptersInStory(storyId, sortedServerIds),
                ...sortedServerIds.map(serverId => fetch(`${BE_HOST}/api/story/${serverId}/${encodeURIComponent(storyId)}`))
            ]);
            const [serverArrResult, chapterCountResult, ...responseResults] = results;
            // serverArrResult
            const serverArr = serverArrResult.status === 'fulfilled' ? serverArrResult.value : [];
            // chapterCountResult
            const chapterCount = chapterCountResult.status === 'fulfilled' ? chapterCountResult.value : 0;
            // responseResults
            let resBody = null;
            let storyServer = null;
            for (let index = 0; index < responseResults.length; index++) {
                const result = responseResults[index];
                if (result.status === 'fulfilled') {
                    const serverId = sortedServerIds[index];
                    const response = result.value;
                    if (response.ok) {
                        resBody = await response.json();
                        storyServer = serverId;
                        break;
                    }
                }
            }
            if (resBody == null) {
                req.session.errorMessage = "Truyện không còn khả thi!";
                if (req.session.history[storyId]) {
                    delete req.session.history[storyId];
                };
                res.redirect('/home');
            }

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
            if (req.session.history[storyId]) {
                render.storyConIndex = (req.session.history[storyId]).chapterIndex;
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