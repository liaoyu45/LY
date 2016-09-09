/// <reference path="../god.js" />
var game = {};
game.all = [];
game.rRows = [];
game.lRows = [];
game.hRows = [];
game.nextHRL = function (hrl, type, direction) {
    var newHRL = game.createHRL(hrl[0], hrl[1], hrl[2]);
    newHRL[(type + 1) % 3] += !hrl.direction ? direction ? 0 : 1 : direction ? -1 : 0;
    newHRL[(type + 2) % 3] += !hrl.direction ? direction ? 1 : 0 : direction ? 0 : -1;
    return newHRL;
};
game.createHRL = function (h, r, l) {
    if (h + r + l > game.count - 1) {
        throw "";
    }
    var hrl = {};
    for (var i = 0; i < 3; i++) {
        hrl[i] = arguments[i];
    }
    Object.defineProperties(hrl, {
        inside: {
            get: function () {
                return this[0] > -1 && this[1] > -1 && this[2] > -1;
            }
        },
        direction: {
            get: function () {
                return game.count - this[0] - this[1] - this[2] == 1;
            }
        },
        sum: {
            value: 1,
            writable: true
        }
    });
    return hrl;
};
game.allRows = [game.hRows, game.rRows, game.lRows];
game.onTagsAdded = null;
game.onNotice = null;
game.startMoving = function (row) {
    var tags = row.children.map(function (e) {
        return e.tag;
    });
    var shaker;
    game.moving = function (d) {
        d = d * 2;
        if (shaker) {
            shaker.moveArray(d);
        } else {
            shaker = god.arr.moveArray(d, tags, game.createTag);
        }
        return shaker.arr;
    };
    game.stopMoving = function () {
        delete game.moving;
        delete game.stopMoving;
        var offset = shaker.front - shaker.frontAdded;
        var len = row.children.length;
        for (var i = 0; i < len; i++) {
            var c = offset > 0 ? row.children[i] : row.children[len - 1 - i];
            c.tag = offset > 0 ? shaker.arr[i + shaker.front] : shaker.arr[shaker.arr.length - 1 - i - shaker.end];
            if (offset) {
                if (Math.abs(offset) < len) {
                    if (offset < 0) {
                        var pre = len - 1 - i + offset;
                        if (pre > -1) {
                            c.sum = row.children[pre].sum;
                        } else {
                            c.sum = 1;
                        }
                    } else {
                        if (i + offset < len) {
                            c.sum = row.children[i + offset].sum;
                        } else {
                            c.sum = 1;
                        }
                    }
                } else {
                    c.sum = 1;
                }
            }
            game.notice(c);
        }
        var siblings = row.index == game.count - 1 ? [row.index - 1, row.index] : row.index == 0 ? [1, 0] : [row.index - 1, row.index, row.index + 1];
        for (var i = 0; i < siblings.length; i++) {
            game.allRows[row.type][siblings[i]].children.forEach(game.collect);
        }
    };
};
game.notice = function (hrl, t) {
    god.safeFunction(game.onNotice).execute(hrl);
};
game.load = function (count, tagsMax) {
    this.count = count;
    this.tagsMax = tagsMax;
    this.createTag = function () {
        return Math.floor(Math.random() * tagsMax);
    }
    for (var i = 0; i < count; i++) {
        for (var j = 0; j < 3; j++) {
            game.allRows[j].push({
                index: i, type: j, children: []
            });
        }
    }
    for (var i = 0; i < count; i++) {
        for (var j = 0; j < count - i; j++) {
            var k = count - 1 - i - j;
            (k > 0 ? [k, k - 1] : [k]).forEach(function (e) {
                var hrl = game.createHRL(i, j, e);
                hrl.tag = game.createTag();
                game.all.push(hrl);
            });
        }
    }
    game.all.sort(function (e0, e1) {
        var h = e0[0] - e1[0];
        if (!h) {
            var r = e1[1] - e0[1];
            if (!r) {
                return e0[2] - e1[2];
            }
            return r;
        }
        return h;
    });
    game.allRows.forEach(function (e, i) {
        game.all.forEach(function (hrl) {
            e[hrl[i]].children.push(hrl);
        });
        var c0 = (i + 2) % 3;
        var c1 = (i + 1) % 3;
        e.forEach(function (r) {
            r.children.sort(function (e0, e1) {
                var h = e0[c0] - e1[c0];
                if (h == 0) {
                    return e1[c1] - e0[c1];
                }
                return h;
            })
        });
    });
};
game.collect1 = function (c, ever) {
    c = game.createHRL(1, 1, 1);
    var _012 = [0, 1, 2];
    var siblings = _012.map(function (e) {
        game.nextHRL(c, e);
    });
    siblings = siblings.filter(function (e) {
        return e.inside;
    });
    if (siblings.length == 3) {
        if (siblings[0].tag == siblings[1].tag && siblings[0].tag == siblings[2].tag) {
            var nn = game.collect2(c, siblings);
            nn.forEach(game.collect1);
        }
    } else {
        siblings.forEach(game.collect1);
    }
};
game.collect = function (center) {
    var arr = [];
    for (var j = 0; j < 3; j++) {
        var n = game.nextHRL(center, j);
        if (!n.inside) {
            return;
        }
        arr.push(n);
    }
    arr = arr.map(function (e) {
        return game.getElement(e);
    });
    if (arr.some(function (e) {
        return e.tag != arr[0].tag;
    })) {
        return;
    }
    god.safeFunction(game.onCollect).execute(center, arr);
    center.sum += 3;
    game.notice(center);
    arr.forEach(function (e) {
        e.sum = 1;
        e.tag = game.createTag();
        game.notice(e);
    });
};
game.getElement = function (hrl) {
    return game.all.filter(function (e) {
        return e[0] == hrl[0] && e[1] == hrl[1] && e[2] == hrl[2];
    })[0];
};
if (location.href.length == 11) {
    game.load(5, 3);
    game.collect(game.rRows[0].children[0]);
}