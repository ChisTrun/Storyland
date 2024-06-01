const express = require('express');
const router = express.Router();

const controller = require('../controller/extension.c');

router.get('/server', controller.GetServer);
router.post('/server/set', controller.SetServerIndex);

module.exports = { router };