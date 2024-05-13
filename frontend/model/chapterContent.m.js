const { BE_HOST } = require('./global/env');

module.exports = class ChapterContent {
    constructor({ content, nextChapUrl, preChapUrl }) {
        this.content = content;
        this.nextChapUrl = nextChapUrl;
        this.preChapUrl = preChapUrl;
    }

    static async getContent(storyName, index) {
        const response = await fetch(`${BE_HOST}api/story/chapter/${storyName}?index=${index}`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return new ChapterContent(data);
    }
};