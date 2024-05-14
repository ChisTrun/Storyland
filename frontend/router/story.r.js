const express = require('express');
const router = express.Router();

const controller = require('../controller/story.c')

router.get('/:storyName', controller.render)

module.exports = { router };