const { BE_HOST } = require('../global/env');

module.exports = class Category {
    constructor({ name, url }) {
        this.name = name;
        this.url = url;
    }

    static async getAll() {
        const response = await fetch(`${BE_HOST}/api/category`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data.map(element => {
            return new Category(element);
        }); 
    }
};