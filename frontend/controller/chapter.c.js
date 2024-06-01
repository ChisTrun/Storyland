const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

const view = 'chapter';
const render = {
    layout: 'main',
    scripts: ['/js/chapter.js'],
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};

module.exports = {
    async render(req, res, next) {
        try {
            const chapterIndex = parseInt(req.params.index);
            const storyServer = req.params.storyServer;
            const storyId = decodeURIComponent(req.params.storyId);
            const serverIndex = req.session.serverIndex;

            const chapterInfoResponse = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}/chapters?page=${chapterIndex+1}&limit=1`);
            const chapterInfoResBody = await chapterInfoResponse.json();            
            const serverResponse = await fetch(`${BE_HOST}/api/server`);
            const serverResBody = await serverResponse.json();

            render.chapterIndex = chapterIndex;
            render.chapterName = chapterInfoResBody.data[0].name;
            render.chapterMaxIndex = chapterInfoResBody.totalPages - 1;
            render.chapterMinIndex = 0;
            render.storyServer = storyServer;
            render.storyId = storyId;
            render.storyName = chapterInfoResBody.data[0].belong.name;
            render.serverArr = serverResBody;

            req.session.history[`${storyId}-server-${storyServer}`] = {
                chapterName: render.chapterName,
                chapterIndex: render.chapterIndex,
                storyId: render.storyId,
                storyServer: render.storyServer,
                storyImageUrl: chapterInfoResBody.data[0].belong.imageUrl,
                storyName: render.storyName,
            };

            render.serverIndex = serverIndex;
            render.isDark = req.session.isDark;
            render.title = `${render.storyName} - ${render.chapterName} | StoryLand`;

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Đọc truyện thất bại", 503, error.message));
        }
    },
    async getAll(req, res, next) {
        try {
            const storyServer = req.params.storyServer;
            const storyId = decodeURIComponent(req.params.storyId);
            
            const response = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}/chapters/all`);
            const resBody = await response.json();

            return res.json(resBody);
        }
        catch (error) {
            console.log(error.message);
            return res.json([]);
        }
    },
    async getContent(req, res, next) {
        try {
            const chapterServer = req.body.chapterServer;
            const index = req.params.index;
            const storyId = decodeURIComponent(req.params.storyId);

            const response = await fetch(`${BE_HOST}/api/story/${chapterServer}/${encodeURIComponent(storyId)}/chapter?index=${index}`);
            const resBody = await response.json();

            const content = `${resBody.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')}`;

            return res.json({'content': content});
        }
        catch (error) {
            console.log(error.message);
            return res.json({'content': '<div class=`text-center`>Nguồn truyện không khả thi!</div>'});
        }
    }
};