const express = require('express');
const router = express.Router();

const storyController = require('../controller/story.c');
const chapterController = require('../controller/chapter.c');

router.get('/:storyId/s:storyServer', storyController.render);
router.get('/:storyId/s:storyServer/chapter/:index', chapterController.render);
router.get('/:storyId/s:storyServer/chapters', chapterController.getAll);
router.post('/:storyId/:index/content', chapterController.getContent);

module.exports = { router };