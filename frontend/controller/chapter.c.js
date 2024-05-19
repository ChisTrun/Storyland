const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');
const view = 'chapter';
const render = {
    layout: 'main',
    scripts: null,
    styles: null,
    header: 'header',
    footer: 'footer',
};
const serverIndex = 0;

module.exports = {
    async render(req, res, next) {
        try {
            const storyId = encodeURIComponent(req.params.storyId);
            const index = parseInt(req.params.index);

            const chapterResponse = await fetch(`${BE_HOST}/api/story/${serverIndex}/${storyId}/chapter?index=${index + 1}`);
            const chapterResBody = await chapterResponse.json();
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${serverIndex}/${storyId}/chapters/all`);
            const chapListResBody = await chapListResponse.json();

            render.content = `${chapterResBody.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')}`;
            render.storyId = storyId;
            render.storyName = chapListResBody[index].belong.name;
            render.index = index;
            render.maxIndex = chapListResBody[chapListResBody.length - 1].index;
            render.minIndex = chapListResBody[0].index;
            render.chapterName = chapListResBody[index].name;
            render.title = `${render.storyName} - ${render.chapterName}`;

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Đọc truyện thất bại", 503, error.message));
        }
    },
};
