const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');

module.exports = {
    async RedirectToHomePage(req, res, next) {
        res.redirect("/home")
    },
    async SetUpCookies(req, res, next) {
        if (req.session.isDark == undefined) {
            req.session.isDark = false;
        }
        if (req.session.serverIndex == undefined) {
            req.session.serverIndex = 0
        }
        if (req.session.history == undefined) {
            req.session.history = {}
        }
        next()
    },
    async ChangeDarkMode(req, res, next) {
        const curMode = req.body.curMode;
        if (curMode === 'light') {
            req.session.isDark = true;
        } else if (curMode === 'dark') {
            req.session.isDark = false;
        } else {
            req.session.isDark = false;
        }
        res.json({ isDark: req.session.isDark });
    },
};