const express = require('express');
const router = express.Router();

const controller = require('../controller/start.c');

router.get('*', controller.setUpCookies);
router.get('/', controller.redirectToHomePage);
router.post('/change-dark-mode', controller.changeDarkMode);
router.post('/error-handler', controller.handleError);

module.exports = { router };