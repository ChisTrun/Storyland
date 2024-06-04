const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');

module.exports = {
    async export(req, res, next) {
        try {
            const storyServer = req.body.storyServer;
            const storyId = decodeURIComponent(req.body.storyId);
            const type = req.body.type;

            const backendUrl = `${BE_HOST}/api/export/${storyServer}/${type}/${encodeURIComponent(storyId)}`;
            return res.send(backendUrl);
        }
        catch (error) {
            console.error(error.message);
            return res.send("");
        }
    },
    async getTypes(req, res, next) {
        try {
            const response = await fetch(`${BE_HOST}/api/export`);
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
};