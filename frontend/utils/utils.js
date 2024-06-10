const { BE_HOST } = require('../global/env');

async function getServerArr() {
    try {
        const response = await fetch(`${BE_HOST}/api/server`);
        if (!response.ok) {
            throw Error();
        }
        const resBody = await response.json();
        return resBody;
    } catch (error) {
        return [];
    }
}

async function countChapter(serverId, storyId) {
    try {
        const response = await fetch(`${BE_HOST}/api/story/${serverId}/${encodeURIComponent(storyId)}/chapters/count`);
        if (!response.ok) {
            throw Error();
        }
        const resBody = await response.json();
        return resBody;
    } catch (error) {
        return 0;
    }
}

async function getNumChaptersInStory(storyId, serverIds) {
    const results = await Promise.allSettled(
        serverIds.map(serverId => countChapter(serverId, storyId))
    );
    const chapterCounts = results.map(result => {
        if (result.status === 'fulfilled') {
            return result.value;
        } else {
            return 0; 
        }
    });
    const maxChapterCount = Math.max(...chapterCounts);
    return maxChapterCount;
}

async function loadChapterContent(serverId, storyId, chapterIndex) {
    try {
        const response = await fetch(`${BE_HOST}/api/story/${serverId}/story/chapter?storyid=${encodeURIComponent(storyId)}&index=${chapterIndex}`);
        if (!response.ok) {
            throw Error();
        }
        const resBody = await response.json();

        const chapterContent = resBody.content.replace(/\r\n/g, '<br>')
            .replace(/\n/g, '<br>')
            .replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
        const chapterName = resBody.name;
        return { 'name': chapterName, 'content': chapterContent };
    }
    catch (error) {
        return { 'name': null, 'content': null };
    }
}

module.exports = { getServerArr, getNumChaptersInStory, loadChapterContent };