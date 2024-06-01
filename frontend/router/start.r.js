const express = require('express');
const router = express.Router();

const controller = require('../controller/start.c');

router.get('/', controller.redirectToHomePage);
router.get('*', controller.setUpCookies);
router.post('/changeDarkMode', controller.changeDarkMode);

module.exports = { router };