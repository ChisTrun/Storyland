const { router } = require('../router/main.r');

module.exports = (app) => {
    app.use(router);
};