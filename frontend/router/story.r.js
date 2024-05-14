const express = require('express');
const router = express.Router();

const storyController = require('../controller/story.c')
const chapterController = require('../controller/chapter.c')

router.get('/:storyName', storyController.render)
router.get('/:storyName/:index', chapterController.render)

module.exports = { router };