module.exports = {
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
};