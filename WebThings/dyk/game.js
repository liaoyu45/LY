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
        for (var i = 0; i < row.children.length; i++) {
            var newTag = shaker.arr[i + shaker.front];
            god.safeFunction(game.onTagChanged).execute(row.children[i], newTag);
            row.children[i].tag = newTag;
        }
    };
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
game.piece = function (r) {
    for (var i = 0; i < r.children.length; i++) {
        var arr = [];
        for (var j = 0; j < 3; j++) {
            var n = game.nextHRL(r.children[i], j);
            if (!n.inside) {
                break;
            }
            arr.push(n);
        }
        if (arr.length < 3) {
            continue;
        }
        if (arr.some(function (e) {
            return e.tag != arr[0].tag;
        })) {
            continue;
        }
    }
};
game.getElement = function (h, r, l) {
    return game.all.filter(function (e) {
        return e[2] == l && e[1] == r && e[0] == h;
    })[0];
};
if (location.href.length == 11) {
    game.load(5, 3);
    game.piece(game.rRows[0]);
}