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
    var hrl = { tag: game.createTag() };
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
        count: {
            value: 1
        }
    });
    return hrl;
};
game.allRows = [game.hRows, game.rRows, game.lRows];
game.onTagsAdded = null;
game.onTagChanged = null;
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
        for (var i = 0; i < row.children.length; i++) {
            var c = row.children[i];
            var newTag = shaker.arr[i + shaker.front];
            if (i + offset < row.children.length) {
                c.count = row.children[i + offset].count;
            } else {
                c.count = 1;
            }
            game.changeTag(c, newTag);
        }
        var siblings = row.index == game.count - 1 ? [row.index - 1] : row.index == 0 ? [1] : [row.index - 1, row.index + 1];
        for (var i = 0; i < siblings.length; i++) {
            game.allRows[row.type][siblings[i]].children.forEach(game.collect);
        }
    };
};
game.changeTag = function (hrl, t) {
    if (typeof t == "undefined") {
        t = game.createTag();
    }
    god.safeFunction(game.onTagChanged).execute(hrl, t);
    hrl.tag = t;
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
            game.all.push(game.createHRL(i, j, k));
            if (k - 1 > -1) {
                game.all.push(game.createHRL(i, j, k - 1));
            }
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
    arr.forEach(function (e) {
        center.count += e.count;
        e.count = 1;
        game.changeTag(e);
    }); 
};
game.getElement = function (hrl) {
    return game.all.filter(function (e) {
        return e[0] == hrl[0] && e[1] == hrl[1] && e[2] == hrl[2];
    })[0];
};
if (location.href.length == 11) {
    game.load(5, 3);
    game.collect(game.rRows[0]);
}