const view = "chapter";
const render = {
    layout: "main",
    scripts: ["/setting.client.js", "/page/home.client.js"],
    styles: null,
    header: "header",
    footer: "footer",
};
const { BE_HOST } = require("../global/env");

module.exports = {
    async render(req, res, next) {
        try {
            const storyName = req.params.storyName;
            const index = req.params.index;

            let response = await fetch(`${BE_HOST}/api/story/chapter/${storyName}?index=${index}`);
            const chapterContent = await response.json();
            
            response = await fetch(`${BE_HOST}/api/story/${storyName}`);
            const chaptersList = await response.json();

            render.storyName = storyName;
            render.index = index;
            render.content = chapterContent.content;
            render.maxIndex = chaptersList[chaptersList.length - 1].index;
            render.minIndex = chaptersList[0].index;
            render.chapterName = chaptersList[index].name;

            return res.render(view, render, null);
        } catch (error) {
            console.error(error);
            next(error);
        }
    },
};
