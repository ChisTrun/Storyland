const express = require('express');
const router = express.Router();

const controller = require('../controller/offlinereading.c');

router.get('/', controller.render);

module.exports = { router };