var session = require('express-session')

module.exports = (app) => {
    app.use(session({
        secret: 'fit-hcmus',
        resave: false,
        saveUninitialized: true,
        cookie: { secure: true }
      }))
}