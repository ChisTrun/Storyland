const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');

const view = 'story';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host: `https://${HOST}:${PORT}`,
};
const perPage = 50;

module.exports = {
    async render(req, res, next) {
        try {
            const curServer = req.params.serverIndex;
            const storyId = decodeURIComponent(req.params.storyId);
            const curPage = parseInt(req.query.page) || 1;

            const storyResponse = await fetch(`${BE_HOST}/api/story/${curServer}/${encodeURIComponent(storyId)}`);
            const storyResBody = await storyResponse.json();
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${curServer}/${encodeURIComponent(storyId)}/chapters?page=${curPage}&limit=${perPage}`);
            const chapListResBody = await chapListResponse.json();
            const totalPages = chapListResBody.totalPages ? chapListResBody.totalPages : 1;

            let desc = storyResBody.description.replace(/\r\n\r\n/g, '<br>')
                .replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
            storyResBody.description = desc.replace(/<br>(\s*&nbsp;)*/g, '<br>');
            Object.assign(render, {
                ...storyResBody
            });
            render.chapters = chapListResBody.data;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.firstIndex = chapListResBody.data.length != 0 ? 1 : undefined;
            render.curServer = curServer;

            if (req.session.history[`${storyId}-server-${curServer}`]) {
                const story = req.session.history[`${storyId}-server-${curServer}`];
                render.conIndex = story.index;
            }

            render.serverIndex = req.session.serverIndex;
            render.isDark = req.session.isDark;
            render.title = `${storyResBody.name} | StoryLand`;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem thông tin truyện thất bại", 503, error.message));
        }
    },
};