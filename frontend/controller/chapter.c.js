const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr, getNumChaptersInStory, loadChapterContent } = require('../utils/utils');

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
            const sortedServerIds = req.session.sortedServerIds;
            const chapterIndex = parseInt(req.params.index);
            const storyServer = req.params.storyServer;
            const storyId = decodeURIComponent(req.params.storyId);

            const response = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();
            const serverArr = await getServerArr();
            const chapterCount = await getNumChaptersInStory(storyId);

            const tempContents = {};
            await Promise.all(sortedServerIds.map(async serverId => {
                try {
                    const serverName = serverArr.find(server => server.id === serverId).name;
                    const { name, content } = await loadChapterContent(serverId, storyId, chapterIndex);
                    tempContents[serverId] = { 
                        'serverId': serverId, 
                        'serverName': serverName, 
                        'chapterName': name, 
                        'chapterContent': content 
                    };
                } catch (error) {
                    console.error(error.message);
                    tempContents[serverId] = { 
                        'serverId': serverId, 
                        'serverName': null, 
                        'chapterName': null, 
                        'chapterContent': null 
                    };
                }
            }));
            const contents = sortedServerIds.map(serverId => tempContents[serverId]);

            render.chapterIndex = chapterIndex;
            render.chapterCount = chapterCount;
            render.chapterContents = contents;
            render.storyId = storyId;
            render.storyName = resBody.name;
            render.curServerId = storyServer;

            req.session.history[`${storyId}-server-${storyServer}`] = {
                chapterName: `Chương ${chapterIndex + 1}`,
                chapterIndex: render.chapterIndex,
                storyId: render.storyId,
                storyServer: storyServer,
                storyImageUrl: resBody.imageUrl,
                storyName: render.storyName,
            };

            render.sortedServerIds = sortedServerIds;
            render.isDark = req.session.isDark;
            render.title = `${render.storyName} - Chương ${chapterIndex + 1} | StoryLand`;

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Xem nội dung chương truyện thất bại!", 500, error.message));
        }
    }
};