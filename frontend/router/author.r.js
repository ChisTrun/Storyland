const express = require('express');
const router = express.Router();

const controller = require('../controller/author.c');

router.get('/:authorName', controller.render);

module.exports = { router };