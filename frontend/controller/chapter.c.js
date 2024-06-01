const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

const view = 'chapter';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        try {
            const curServer = parseInt(req.params.serverIndex);
            const storyId = decodeURIComponent(req.params.storyId);
            const index = parseInt(req.params.index);

            const chapterResponse = await fetch(`${BE_HOST}/api/story/${curServer}/${encodeURIComponent(storyId)}/chapter?index=${index}`);
            const chapterResBody = await chapterResponse.json();
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${curServer}/${encodeURIComponent(storyId)}/chapters/all`);
            const chapListResBody = await chapListResponse.json();
            const serverResponse = await fetch(`${BE_HOST}/api/server`);
            const serverResBody = await serverResponse.json();

            render.content = `${chapterResBody.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')}`;
            render.curServer = curServer;
            render.storyId = storyId;
            render.serverArr = serverResBody;
            render.index = index;
            render.storyName = chapListResBody[index-1].belong.name;
            render.maxIndex = chapListResBody[chapListResBody.length - 1].index + 1;
            render.minIndex = chapListResBody[0].index + 1;
            render.chapterName = chapListResBody[index-1].name;
            render.chaptersList = chapListResBody;

            req.session.history[`${storyId}-server-${curServer}`] = {
                id: storyId,
                server: curServer,
                imageUrl: chapListResBody[index-1].belong.imageUrl,
                storyName: render.storyName,
                chapterName: render.chapterName,
                index: index,
            };

            render.serverIndex = req.session.serverIndex;
            render.isDark = req.session.isDark;
            render.title = `${render.storyName} - ${render.chapterName} | StoryLand`;

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Đọc truyện thất bại", 503, error.message));
        }
    },
};