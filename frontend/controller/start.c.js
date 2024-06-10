const { getServerArr } = require('../utils/utils');

module.exports = {
    async redirectToHomePage(req, res, next) {
        res.redirect("/home");
    },
    async setUpCookies(req, res, next) {
        if (req.session.isDark == undefined) {
            req.session.isDark = false;
        }
        if (req.session.history == undefined) {
            req.session.history = {};
        }
        try {
            if (req.session.sortedServerIds == undefined) {
                const serverArr = await getServerArr();
                req.session.sortedServerIds = serverArr.map(server => server.id);;
            }
        }
        catch (error) {
            req.session.sortedServerIds = [];
        }
        next();
    },
    async changeDarkMode(req, res, next) {
        const curMode = req.body.curMode;
        req.session.isDark = (curMode == 'light') ?  true : false;
        res.json({ isDark: req.session.isDark });
    },
    async handleError(req, res, next) {
        const errorMessage = req.body.error;
        req.session.errorMessage = errorMessage;
        res.end();
    }
};