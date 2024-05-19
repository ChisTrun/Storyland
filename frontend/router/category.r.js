const express = require('express');
const router = express.Router();

const controller = require('../controller/category.c')

router.get('/:categoryName', controller.render)

module.exports = { router };