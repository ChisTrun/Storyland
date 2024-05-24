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
            const curServer = parseInt(req.params.server);
            const storyId = encodeURIComponent(req.params.storyId);
            const index = parseInt(req.params.index);
            const chapterResponse = await fetch(`${BE_HOST}/api/story/${curServer}/${storyId}/chapter?index=${index + 1}`);
            const chapterResBody = await chapterResponse.json();
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${curServer}/${storyId}/chapters/all`);
            const chapListResBody = await chapListResponse.json();
            render.content = `${chapterResBody.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')}`;

            const serverResponse = await fetch(`${BE_HOST}/api/server`);
            const serverResBody = await serverResponse.json();
            render.serverIndex = req.session.serverIndex
            render.curServer = curServer
            render.storyId = storyId;
            render.serverArr = serverResBody
            render.storyName = chapListResBody[index].belong.name;
            render.index = index;
            render.maxIndex = chapListResBody[chapListResBody.length - 1].index;
            render.minIndex = chapListResBody[0].index;
            render.chapterName = chapListResBody[index].name;
            render.title = `${render.storyName} - ${render.chapterName}`;
            render.isDark = req.session.isDark;

            req.session.history[`${storyId}-${req.session.serverIndex}`] = {
                id: storyId,
                imageUrl: chapListResBody[index].belong.imageUrl,
                storyName: render.storyName,
                chapterName: render.chapterName,
                server: curServer,
                index: index,
            }

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Đọc truyện thất bại", 503, error.message));
        }
    },
};
