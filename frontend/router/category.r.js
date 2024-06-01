const express = require('express');
const router = express.Router();

const controller = require('../controller/category.c');

router.get('/:serverIndex/all', controller.getAll);
router.get('/:serverIndex/:categoryName', controller.render);

module.exports = { router };