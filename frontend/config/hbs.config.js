const { express, expressHbs } = require('../global/lib');
const hbsHelpers = require('../helper/hbs.helper');

module.exports = (app) => {
    app.engine('hbs', expressHbs.engine({
        extname: 'hbs',
        layoutsDir: './view/layout',
        defaultLayout: 'plain',
        partialsDir: './view/partials',
        helpers: hbsHelpers,
    }));
    app.set('view engine', 'hbs');
    app.set('views', './view/page');

    app.use(express.static('./view/script'));
    app.use(express.static('./view/style'));
    app.use(express.static('./public'));
}