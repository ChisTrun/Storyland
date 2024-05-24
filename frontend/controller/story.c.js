const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST ,HOST,PORT} = require('../global/env');
const view = 'story';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
    host : `https://${HOST}:${PORT}`,
};
const perPage = 50;


module.exports = {
    async render(req, res, next) {
        try {
            const serverIndex = req.session.serverIndex;
            const storyId = encodeURIComponent(req.params.storyId);
            const curPage = parseInt(req.query.page) || 1;

            const storyResponse = await fetch(`${BE_HOST}/api/story/${serverIndex}/${storyId}`);
            const storyResBody = await storyResponse.json();
           
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${serverIndex}/${storyId}/chapters?page=${curPage}&limit=${perPage}`);
            
            const chapListResBody = await chapListResponse.json();
            const totalPages = chapListResBody.totalPages ?  chapListResBody.totalPages : 1;

            storyResBody.description = storyResBody.description.replace(/\r\n\r\n/g, '<br>')
                .replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
            storyResBody.description = storyResBody.description.replace(/<br>(\s*&nbsp;)*/g, '<br>');
            
            Object.assign(render, {
                ...storyResBody
            });

            render.serverIndex = serverIndex
            render.chapters = chapListResBody.data;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.firstIndex = 0;
            render.title = storyResBody.name;
            render.isDark = req.session.isDark;

            if (req.session.history[`${storyId}-${serverIndex}`]) {
                const story = req.session.history[`${storyId}-${serverIndex}`];
                render.conIndex = story.index;
            }

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem truyện thất bại", 503, error.message));
        }
    },
};
