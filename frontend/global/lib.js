const express = require('express');
const fs = require('fs');
const https = require('https');
const expressHbs = require('express-handlebars');
const cookieParser = require('cookie-parser');

module.exports = { express, expressHbs, cookieParser, https, fs };