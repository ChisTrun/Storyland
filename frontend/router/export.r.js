const express = require('express');
const router = express.Router();

const controller = require('../controller/export.c');

router.get('/', controller.getTypes);
router.post('/', controller.export);

module.exports = { router };