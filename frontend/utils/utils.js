const { BE_HOST } = require('../global/env');

async function getServerArr() {
    try {
        const response = await fetch(`${BE_HOST}/api/server`);
        if (!response.ok) {
            const errorMessage = await response.text();
            throw Error(errorMessage);
        }
        const resBody = await response.json();
        return resBody;
    } catch (error) {
        console.error(error.message);
        return [];
    }
}

async function countChapter(serverId, storyId) {
    try {
        const response = await fetch(`${BE_HOST}/api/story/${serverId}/${encodeURIComponent(storyId)}/chapters/count`);
        if (!response.ok) {
            const errorMessage = await response.text();
            throw Error(errorMessage);
        }
        const resBody = await response.json();
        return resBody;
    } catch (error) {
        // console.error(error.message);
        return 0;
    }
}

async function getNumChaptersInStory(storyId) {
    const serverArr = await getServerArr();
    const chapterCounts = await Promise.all(
        serverArr.map(server =>
            countChapter(server.id, storyId).catch(error => {
                console.error(error.message);
                return 0;
            })
        )
    );
    const maxChapterCount = Math.max(...chapterCounts);
    return maxChapterCount;
}

async function loadChapterContent(serverId, storyId, chapterIndex) {
    try {
        const response = await fetch(`${BE_HOST}/api/story/${serverId}/story/chapter?storyid=${encodeURIComponent(storyId)}&index=${chapterIndex}`);
        if (!response.ok) {
            const errorMessage = await response.text();
            throw Error(errorMessage);
        }
        const resBody = await response.json();

        const chapterContent = resBody.content.replace(/\r\n/g, '<br>')
            .replace(/\n/g, '<br>')
            .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
        const chapterName = resBody.chapterName;
        return { 'name': chapterName, 'content': chapterContent };
    }
    catch (error) {
        // console.error(error.message);
        return { 'name': null, 'content': null };
    }
}

module.exports = { getServerArr, getNumChaptersInStory, loadChapterContent };