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

            const results = await Promise.allSettled([
                fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}`),
                getServerArr(),
                getNumChaptersInStory(storyId, sortedServerIds),
                ...sortedServerIds.map(serverId => loadChapterContent(serverId, storyId, chapterIndex))
            ]);
            const [responseResult, serverArrResult, chapterCountResult, ...chapterContentsResults] = results;
            // responseResult
            if (responseResult.status !== 'fulfilled') {
                throw Error(`Error fetching request: ${responseResult.hasOwnProperty("reason") ? responseResult.reason : "pending"}`);
            }
            const response = responseResult.value;
            if (!response.ok) {
                const errorMessage = await response.text();
                next(new ErrorDisplay("Xem nội dung chương truyện thất bại!", response.status, errorMessage));
            }
            const resBody = await response.json();
            // serverArrResult
            const serverArr = serverArrResult.status === 'fulfilled' ? serverArrResult.value : [];
            // chapterCountResult
            const chapterCount = chapterCountResult.status === 'fulfilled' ? chapterCountResult.value : 0;
            // chapterContentsResults
            const contents = [];
            chapterContentsResults.forEach((result, index) => {
                const serverId = sortedServerIds[index];
                const serverName = serverArr.find(server => server.id === serverId)?.name || null;
                if (result.status === 'fulfilled') {
                    const { name, content } = result.value;
                    contents.push({
                        'serverId': serverId,
                        'serverName': serverName,
                        'chapterName': name,
                        'chapterContent': content
                    });
                } else {
                    contents.push({
                        'serverId': serverId,
                        'serverName': serverName,
                        'chapterName': null,
                        'chapterContent': null
                    });
                }
            });

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
            next(new ErrorDisplay("Xem nội dung chương truyện thất bại!", 503, error.message));
        }
    }
};