const express = require('express');
const router = express.Router();

const controller = require('../controller/start.c');

router.get('*', controller.setUpCookies);
router.get('/', controller.redirectToHomePage);
router.post('/changeDarkMode', controller.changeDarkMode);
router.post('/errorHandler', controller.handleError);

module.exports = { router };