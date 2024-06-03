const express = require('express');
const router = express.Router();

const controller = require('../controller/author.c');

router.get('/:authorName/s:storyServer', controller.render);

module.exports = { router };