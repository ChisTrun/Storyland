const { BE_HOST } = require('../global/env');

module.exports = {
    async export(req, res, next) {
        try {
            const storyServer = req.body.storyServer;
            const storyId = decodeURIComponent(req.body.storyId);
            const type = req.body.type;
            const exportUrl = `${BE_HOST}/api/export/${storyServer}/${type}/${encodeURIComponent(storyId)}`;
            res.json({ url: exportUrl });
        }
        catch (error) {
            console.error(error.message);
            res.status(503).send(error.message);
        }
    },
    async getTypes(req, res, next) {
        try {
            const response = await fetch(`${BE_HOST}/api/export`);
            if (!response.ok) {
                throw Error();
            }
            const resBody = await response.json();
            res.json(resBody);
        }
        catch (error) {
            res.json([]);
        }
    },
};