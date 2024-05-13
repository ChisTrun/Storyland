const { BE_HOST } = require('./global/env');

module.exports = class Chapter {
    constructor({ index, name, url }) {
        this.index = index;
        this.name = name;
        this.url = url;
    }

    static async getAll(storyName) {
        const response = await fetch(`${BE_HOST}api/story/${storyName}`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data.map(element => {
            return new Chapter(element);
        }); 
    }
};