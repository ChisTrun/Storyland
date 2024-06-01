const express = require('express');
const router = express.Router();

const controller = require('../controller/start.c');

router.get('/', controller.redirectToHomePage);
router.post('/changeDarkMode', controller.changeDarkMode);
router.get('*', controller.setUpCookies);

module.exports = { router };