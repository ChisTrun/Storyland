const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');

module.exports = {
    async getServer(req, res, next) {
        try {
            const response = await fetch(`${BE_HOST}/api/server`);
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
    async setServerIndex(req, res, next) {
        req.session.serverIndex = req.body.index;
        return res.end();
    },
};