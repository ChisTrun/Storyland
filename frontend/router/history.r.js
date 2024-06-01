const express = require('express');
const router = express.Router();

const controller = require('../controller/history.c');

router.get('/', controller.render);
router.post('/delete', controller.delete);

module.exports = { router };