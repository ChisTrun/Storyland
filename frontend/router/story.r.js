const express = require('express');
const router = express.Router();

const storyController = require('../controller/story.c');
const chapterController = require('../controller/chapter.c');

router.get('/:serverIndex/:storyId', storyController.render);
router.get('/:serverIndex/:storyId/:index', chapterController.render);

module.exports = { router };