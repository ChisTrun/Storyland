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
            const storyId = decodeURIComponent(req.params.storyId);

            const results = await Promise.allSettled([
                ...sortedServerIds.map(serverId => fetch(`${BE_HOST}/api/story/${serverId}/${encodeURIComponent(storyId)}`)),
                getServerArr(),
                getNumChaptersInStory(storyId, sortedServerIds),
                ...sortedServerIds.map(serverId => loadChapterContent(serverId, storyId, chapterIndex))
            ]);
            const storyDetailResults = results.slice(0, sortedServerIds.length);
            const [serverArrResult, chapterCountResult, ...chapterContentsResults] = results.slice(sortedServerIds.length);
            // storyDetailResults
            let resBody = null;
            let storyServer = null;
            for (let index = 0; index < storyDetailResults.length; index++) {
                const result = storyDetailResults[index];
                if (result.status === 'fulfilled') {
                    const serverId = sortedServerIds[index];
                    const response = result.value;
                    if (response.ok) {
                        resBody = await response.json();
                        storyServer = serverId;
                        break; 
                    }
                }
            }
            if (resBody == null) {
                req.session.errorMessage = "Truyện không còn khả thi!";
                if (req.session.history[storyId]) {
                    delete req.session.history[storyId]
                };
                res.redirect('/home');
            }
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

            req.session.history[storyId] = {
                chapterName: `Chương ${chapterIndex + 1}`,
                chapterIndex: render.chapterIndex,
                storyId: render.storyId,
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