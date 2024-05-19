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

module.exports = {
    async render(req, res, next) {
        try {
            storyId = encodeURIComponent(req.params.storyId);
            const index = parseInt(req.params.index);

            const chapterResponse = await fetch(`${BE_HOST}/api/story/${storyId}/chapter?index=${index + 1}`);
            const chapterData = await chapterResponse.json();
            const chapListResponse = await fetch(`${BE_HOST}/api/story/${storyId}/chapters/all`);
            const chapListData = await chapListResponse.json();

            render.content = `${chapterData.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')}`;
            render.storyId = storyId;
            render.storyName = chapListData[index].belong.name;
            render.index = index;
            render.maxIndex = chapListData[chapListData.length - 1].index;
            render.minIndex = chapListData[0].index;
            render.chapterName = chapListData[index].name;
            render.title = `${render.storyName} - ${render.chapterName}`;

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Đọc truyện thất bại", 503, error.message));
        }
    },
};
