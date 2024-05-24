const express = require('express');
const router = express.Router();

const controller = require('../controller/start.c')

router.get('/', controller.RedirectToHomePage)
router.post('/changeDarkMode', controller.ChangeDarkMode)
router.get('*', controller.SetUpCookies)

module.exports = { router };