const { ErrorDisplay } = require('../middleware/error');
const { BE_HOST } = require('../global/env');

module.exports = {
    async RedirectToHomePage(req,res,next) {
        res.redirect("/home")
    },
    async SetUpCookies(req, res, next) {
       if(req.session.serverIndex == undefined) {
            req.session.serverIndex = 0
       }
       next()
    },
};