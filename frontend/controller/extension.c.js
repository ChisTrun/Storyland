const { getServerArr } = require('../utils/utils');

module.exports = {
    async getServerArr(req, res, next) {
        try {
            const serverArr = await getServerArr();
            res.json(serverArr);
        }
        catch (error) {
            res.json([]);
        }
    },
    async setServerIds(req, res, next) {
        req.session.sortedServerIds = req.body.sortedServerIds;
        res.end();
    },
};