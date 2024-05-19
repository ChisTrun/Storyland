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

module.exports = {
    async render(req, res, next) {
        try {
            storyId = encodeURIComponent(req.params.storyId);
            let curPage = parseInt(req.query.page) || 1;

            const storyResponse = await fetch(`${BE_HOST}/api/story/${storyId}`);
            const storyData = await storyResponse.json();
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${storyId}/chapters/all`);
            const chapListData = await chapListResponse.json();

            const totalPages = chapListData.length == 0 ? 1 : Math.ceil(chapListData.length / perPage);
            curPage = Math.min(Math.max(parseInt(curPage), 1), totalPages);

            storyData.description = storyData.description.replace(/\r\n\r\n/g, '<br>')
                .replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
            Object.assign(render, {
                ...storyData
            });
            render.chapters = chapListData.slice((curPage - 1) * perPage, curPage * perPage);
            render.curPage = curPage;
            render.totalPages = totalPages;
            render.totalChapters = chapListData ? chapListData.length : 0;
            render.firstIndex = chapListData ? chapListData[0].index : null;
            render.lastIndex = chapListData ? chapListData[chapListData.length - 1].index : null;
            render.title = storyData.name;

            return res.render(view, render, null);
        }
        catch (error) {
            next(new ErrorDisplay("Xem truyện thất bại", 503, error.message));
        }
    },
};
