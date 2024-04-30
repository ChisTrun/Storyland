const { express } = require('../global/lib');
const router = express.Router();

// >>>> =============================================
// Routes
// <<<< =============================================

const { router: homeRouter } = require('./home.r');

router.use('/home', homeRouter);

// >>>> =============================================
// Error
// <<<< =============================================

const error = require('../middleware/error');

router.use(
    error.logDisplay,
    error.xmlhttpError,
    error.predictedErrorPageDisplay,
    error.finalHandler,
);

router.use((req, res, next) => {
    return res.status(404).render('error', {
        layout: 'plain',
        err_name: '404 Not found!',
        err_message: `Sorry can't find that`,
    }, null);
});

module.exports = { router };