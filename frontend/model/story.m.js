const { BE_HOST } = require('../global/env');

module.exports = class Story {
    constructor(name, url) {
        this.name = name;
        this.url = url;
    }
    
    static async getStoriesOfCategory(categoryName) {
        const response = await fetch(`${BE_HOST}/api/category/${categoryName}`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data.map(element => {
            return new Story(element.name, element.url);
        }); 
    }

    static async getStoryByName(storyName) {
        const response = await fetch(`${BE_HOST}/api/search/truyen/${storyName}`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data.map(element => {
            return new Story(element.name, element.url);
        }); 
    }
};