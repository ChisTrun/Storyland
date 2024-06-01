const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');

module.exports = {
    async GetServer(req, res, next) {
        try {
            const response = await fetch(`${BE_HOST}/api/server`);
            const resBody = await response.json();
            res.json(resBody);
        }
        catch (error) {
            next(new ErrorDisplay("Không thể lấy danh sách server", 503, error.message));
        }
    },
    async SetServerIndex(req, res, next) {
        try {
            req.session.serverIndex = req.body.index;
            res.end();
        }
        catch (error) {
            next(new ErrorDisplay("Không thể set server index", 503, error.message));
        }
    },
};