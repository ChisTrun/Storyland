const express = require('express');
const router = express.Router();

const controller = require('../controller/history.c');

router.get('/all', controller.render);
router.post('/del', controller.delete);

module.exports = { router };