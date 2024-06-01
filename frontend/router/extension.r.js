const express = require('express');
const router = express.Router();

const controller = require('../controller/extension.c');

router.get('/server', controller.getServer);
router.post('/server/set', controller.setServerIndex);

module.exports = { router };