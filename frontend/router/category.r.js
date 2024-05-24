const express = require('express');
const router = express.Router();

const controller = require('../controller/category.c');

router.get('/all', controller.getAll);
router.get('/:categoryName', controller.render);

module.exports = { router };