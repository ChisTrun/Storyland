module.exports = {
    encodeUrl: function (str) {
        if (str.indexOf('/') >= 0) {
            return encodeURIComponent(str);
        }
        return str;
    },
    isDefined: function (value, block) {
        return value !== undefined ? block.fn(this) : block.inverse(this);
    },
    getLength: function (list) {
        return list ? list.length : 0;
    },
    toLoud: function (text) {
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
    plus: function (a, b) {
        const sum = parseInt(a) + parseInt(b);
        return sum;
    },
    ifEqual: function (a, b, block) {
        if (a == b) {
            return block.fn(this);
        }
        return block.inverse(this);
    },
    ifNotEqual: function (a, b, block) {
        if (a != b) {
            return block.fn(this);
        }
        return block.inverse(this);
    },
    isLess: function (a, b) {
        return (parseFloat(a) < parseFloat(b));
    },
    genPagination: function (curPage, totalPages, block) {
        const pagination = [];
        const startPage = Math.max(1, curPage - 2);
        const endPage = Math.min(totalPages, startPage + 4);

        for (let i = startPage; i <= endPage; i++) {
            pagination.push(block.fn(i));
        }
        return pagination.length == 1 ? '' : pagination.join(' ');
    },
};