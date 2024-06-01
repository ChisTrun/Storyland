module.exports = {
    async redirectToHomePage(req, res, next) {
        res.redirect("/home");
    },
    async setUpCookies(req, res, next) {
        if (req.session.isDark == undefined) {
            req.session.isDark = false;
        }
        if (req.session.serverIndex == undefined) {
            req.session.serverIndex = 0;
        }
        if (req.session.history == undefined) {
            req.session.history = {};
        }
        next();
    },
    async changeDarkMode(req, res, next) {
        const curMode = req.body.curMode;
        curMode === 'light' ? req.session.isDark = true : req.session.isDark = false;
        return res.json({ isDark: req.session.isDark });
    },
};