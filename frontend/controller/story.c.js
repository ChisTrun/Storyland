const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');
const view = 'story';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
};
const perPage = 50;
const serverIndex = 0;

module.exports = {
    async render(req, res, next) {
        try {
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
            Object.assign(render, {
                ...storyResBody
            });
            render.chapters = chapListResBody.data;
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.firstIndex = 0;
            render.title = storyResBody.name;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem truyện thất bại", 503, error.message));
        }
    },
};
