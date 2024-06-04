const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST, HOST, PORT } = require('../global/env');
const { getServerArr } = require('../utils/utils');

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

            const chapterResponse = await fetch(`${BE_HOST}/api/story/${storyServer}/story/chapter?storyid=${encodeURIComponent(storyId)}&index=${chapterIndex}`);
            if (!chapterResponse.ok) {
                const errorMessage = await chapterResponse.text();
                throw Error(errorMessage);
            }
            const chapterResBody = await chapterResponse.json();
            const storyResponse = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}`);
            if (!storyResponse.ok) {
                const errorMessage = await storyResponse.text();
                throw Error(errorMessage);
            }
            const storyResBody = await storyResponse.json();
            const serverArr = await getServerArr();

            render.chapterIndex = chapterIndex;
            render.chapterName = chapterResBody.chapterName;
            if (chapterResBody.nextChapID != undefined && chapterResBody.nextChapID != 0 && chapterResBody.nextChapID != null && chapterResBody.nextChapID != "") {
                render.chapterNextIndex = chapterIndex + 1;
            }
            else {
                render.chapterNextIndex = undefined;
            }
            if (chapterResBody.prevChapID != undefined && chapterResBody.prevChapID != 0 && chapterResBody.prevChapID != null && chapterResBody.prevChapID != "") {
                render.chapterPrevIndex = chapterIndex - 1;
            }
            else {
                render.chapterPrevIndex = undefined;
            }
            render.chapterContent = chapterResBody.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');;
            render.storyServer = storyServer;
            render.storyId = storyId;
            render.storyName = storyResBody.name;
            render.serverArr = serverArr;

            req.session.history[`${storyId}-server-${storyServer}`] = {
                chapterName: render.chapterName,
                chapterIndex: render.chapterIndex,
                storyId: render.storyId,
                storyServer: render.storyServer,
                storyImageUrl: storyResBody.imageUrl,
                storyName: render.storyName,
            };

            render.serverIndex = serverIndex;
            render.isDark = req.session.isDark;
            render.title = `${render.storyName} - ${render.chapterName} | StoryLand`;

            return res.render(view, render, null);
        } catch (error) {
            next(new ErrorDisplay("Xem nội dung chương truyện thất bại!", 500, error.message));
        }
    },
    async getAll(req, res, next) {
        try {
            const storyServer = req.params.storyServer;
            const storyId = decodeURIComponent(req.params.storyId);

            const response = await fetch(`${BE_HOST}/api/story/${storyServer}/${encodeURIComponent(storyId)}/chapters/all`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();

            return res.json(resBody);
        }
        catch (error) {
            console.error(error.message);
            return res.json([]);
        }
    },
    async getContent(req, res, next) {
        try {
            const chapterServer = req.body.chapterServer;
            const chapterIndex = req.params.index;
            const storyId = decodeURIComponent(req.params.storyId);

            const response = await fetch(`${BE_HOST}/api/story/${chapterServer}/story/chapter?storyid=${encodeURIComponent(storyId)}&index=${chapterIndex}`);
            if (!response.ok) {
                const errorMessage = await response.text();
                throw Error(errorMessage);
            }
            const resBody = await response.json();

            const content = resBody.content.replace(/\r\n/g, '<br>')
                .replace(/\n/g, '<br>')
                .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');

            return res.json({ 'content': content });
        }
        catch (error) {
            console.error(error.message);
            return res.status(400).send("Bad Request");
        }
    },
};