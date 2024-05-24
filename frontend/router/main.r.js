const { express } = require('../global/lib');
const router = express.Router();

// >>>> =============================================
// Routes
// <<<< =============================================

const { router: homeRouter } = require('./home.r');
const { router: storyRouter } = require('./story.r');
const { router: searchRouter } = require('./search.r');
const { router: authorRouter } = require('./author.r');
const { router: categoryRouter } = require('./category.r');
const { router: extensionRouter } = require('./extension.r');
const { router: startRouter } = require('./start.r');
const { router: historyRouter } = require('./history.r');


router.use("/",startRouter)
router.use('/home', homeRouter);
router.use('/story', storyRouter);
router.use('/search', searchRouter);
router.use('/author', authorRouter);
router.use('/category', categoryRouter);
router.use('/extension', extensionRouter);
router.use('/history', historyRouter);


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
        title: "Not found!",
        code: '404',
        message: `Sorry can't find that!`,
    }, null);
});

module.exports = { router };