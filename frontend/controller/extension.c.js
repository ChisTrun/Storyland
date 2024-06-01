const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');

module.exports = {
    async getServer(req, res, next) {
        try {
            const response = await fetch(`${BE_HOST}/api/server`);
            const resBody = await response.json();
            return res.json(resBody);
        }
        catch (error) {
            next(new ErrorDisplay("Không thể lấy danh sách server", 503, error.message));
        }
    },
    async setServerIndex(req, res, next) {
        try {
            req.session.serverIndex = req.body.index;
            return res.end();
        }
        catch (error) {
            next(new ErrorDisplay("Không thể set server index", 503, error.message));
        }
    },
};