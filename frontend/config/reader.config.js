const { express, cookieParser } = require('../global/lib');

module.exports = (app) => {
    app.use(cookieParser());
    app.use(express.urlencoded({ extended: true }));
    app.use(express.json());
}