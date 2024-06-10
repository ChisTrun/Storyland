const express = require('express');
const router = express.Router();

const controller = require('../controller/extension.c');

router.get('/server', controller.getServerArr);
router.post('/server/set', controller.setServerIds);

module.exports = { router };