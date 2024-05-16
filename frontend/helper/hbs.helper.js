const { HOST, PORT } = require('../global/env');

module.exports = {
    getUrlFromPath: function (path) {
        return `https://${HOST}:${PORT}/${path}`;
    },
    length: function (list) {
        return list ? list.length : 0;
    },
    toLoud: function(text) {
        return text.toUpperCase();
    },
    limitText: function (text, maxLength) {
        if (text.length <= maxLength) {
            return text;
        } else {
            return text.substring(0, maxLength) + '...';
        }
    },
    include: function (elem, list) {
        if (list.indexOf(elem) > -1) {
            return true;
        }
        return false;
    },
    loop: function (from, to, block) {
        let res = '';
        for (let i = from; i <= to; ++i)
            res += block.fn(i);
        return res;
    },
    ifEqual: function (a, b, block) {
        if (a == b) {
            return block.fn(this);
        }
        return block.inverse(this);
    },
    plus: function (a, b) {
        const sum = parseInt(a) + parseInt(b);
        return sum;
    },
    generatePagination: function (curPage, totalPages, block) {
        const pagination = [];
        let startShowPage = Math.max(1, curPage - 2);
        let endShowPage = Math.min(totalPages, startShowPage + 4);

        for (let i = startShowPage; i <= endShowPage; i++) {
            pagination.push(block.fn(i));
        }

        pagination <= 1 ? [] : pagination;
        return pagination.join(' ');
    },
};