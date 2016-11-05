/// <reference path="../god.js" />
(function () {
    var gData = {};
    gData.last = -1;
    gData.createHRL = function (h, r, l) {
        if (h + r + l > gData.count - 1) {
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
            x: {
                get: function () {
                    return (gData.count - 1 - (this[1] - this[2])) / 2;
                }
            },
            y: {
                get: function () {
                    return gData.count - this[0] - 1;
                }
            },
            direction: {
                get: function () {
                    return gData.count - this[0] - this[1] - this[2] == 1;
                },
                enumerable: true
            },
            sum: {
                value: 0,
                writable: true
            },
            text: {
                get: function () {
                    return JSON.stringify({ hrl: this[0] + "," + this[1] + "," + this[2], tag: this.tag, sum: this.sum });
                }
            },
            fullSiblings: {
                get: function () {
                    var arr = [];
                    for (var i = 0; i < 3; i++) {
                        var newHRL = gData.createHRL(this[0], this[1], this[2]);
                        newHRL[(i + 1) % 3] += this.direction ? 0 : 1;
                        newHRL[(i + 2) % 3] += this.direction ? -1 : 0;
                        arr.push(newHRL);
                    }
                    return arr;
                }
            },
            siblings: {
                get: function () {
                    var arr = [];
                    for (var i = 0; i < 3; i++) {
                        var newHRL = gData.createHRL(this[0], this[1], this[2]);
                        newHRL[(i + 1) % 3] += this.direction ? 0 : 1;
                        newHRL[(i + 2) % 3] += this.direction ? -1 : 0;
                        if (newHRL.inside) {
                            var e = gData.getElement(newHRL);
                            if (e) {
                                arr.push(e);
                            }
                        }
                    }
                    return arr;
                }
            },
            active: {
                get: function () {
                    var s = this.siblings;
                    if (s.length != 3) {
                        return false;
                    }
                    var same = !s.some(function (e) { return e.tag != this.tag; }, this);
                    return same;
                }
            }
        });
        hrl.getData = function () {
            var r = {};
            r.direction = this.direction;
            r[0] = this[0];
            r[1] = this[1];
            r[2] = this[2];
            r.tag = this.tag;
            r.sum = this.sum;
            r.text = this.text;
            return r;
        };
        return hrl;
    };
    gData.getElement = function (hrl) {
        return game.all.filter(function (e) {
            return e[0] == hrl[0] && e[1] == hrl[1] && e[2] == hrl[2];
        })[0];
    };
    var game = {};
    god.addEventListener(game, "notice", "startmoving", "ready", "collecting", "lost", "movingstopped");
    game.all = [];
    game.allRows = [[], [], []];
    game.load = function (count, tagsMax, sumMax) {
        gData.count = count;
        Object.defineProperties(this, {
            count: { get: function () { return count; } },
            tagsMax: { get: function () { return tagsMax; } },
            sumMax: { get: function () { return sumMax; } }
        })
        gData.createTag = function () {
            return Math.floor(Math.random() * tagsMax);
        }
        for (var i = 0; i < count; i++) {
            for (var j = 0; j < 3; j++) {
                game.allRows[j].push({
                    index: i, type: j, children: []
                });
            }
        }
        game.allRows.forEach(function (e) {
            e.forEach(function (ee, i) {
                ee.siblings = [];
                [i - 1, i + 1].forEach(function (eee) {
                    var s = game.allRows[this.type][eee];
                    if (s) {
                        this.siblings.push(s);
                    }
                }, ee);
            });
        });
        for (var i = 0; i < count; i++) {
            for (var j = 0; j < count - i; j++) {
                var k = count - 1 - i - j;
                (k > 0 ? [k, k - 1] : [k]).forEach(function (e) {
                    var hrl = gData.createHRL(i, j, e);
                    hrl.tag = gData.createTag();
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
        function loaded() {
            var ed;
            for (var i = 0; i < game.all.length; i++) {
                if (game.collectOne(game.all[i])) {
                    ed = true;
                }
            }
            if (ed) {
                loaded();
            }
        }
        loaded();
        game.all.forEach(function () {
            arguments[0].sum = 0;
        });
        delete game.load;
    };
    game.start2move = function (row) {
        if (game.move) {
            return;
        }
        game.noticestartmoving();
        var tags = row.children.map(function (e) {
            return e.tag;
        });
        var shaker;
        game.move = function (d) {
            d = d * 2;
            if (shaker) {
                shaker.moveArray(d);
            } else {
                shaker = god.arr.moveArray(d, tags, gData.createTag);
            }
            return shaker;
        };
        game.stopMoving = function () {
            game.noticemovingstopped();
            var offset = shaker.front - shaker.frontAdded;
            if (!offset) {
                game.noticeready();
                delete game.move;
                delete game.stopMoving;
                return;
            }
            var len = row.children.length;
            var oldData = row.children.map(function (e) {
                return e.getData();
            });
            for (var i = 0; i < len; i++) {
                var c = offset > 0 ? row.children[i] : row.children[len - 1 - i];
                var ot = c.tag, os = c.sum;
                if (Math.abs(offset) < len) {
                    if (offset < 0) {
                        var pre = len - 1 - i + offset;
                        if (pre > -1) {
                            c.sum = row.children[pre].sum;
                        } else {
                            c.sum = 0;
                        }
                    } else {
                        if (i + offset < len) {
                            c.sum = row.children[i + offset].sum;
                        } else {
                            c.sum = 0;
                        }
                    }
                } else {
                    c.sum = 0;
                }
                c.tag = offset > 0 ? shaker.arr[i + shaker.front] : shaker.arr[shaker.arr.length - 1 - i - shaker.end];
                game.notice(c, ot, os);
            }
            for (var i = 0; i < Math.abs(offset) ; i++) {
                var ci = offset > 0 ? i : len - 1 - i;
                var c = oldData[ci];
                if (c && c.sum) {
                    game.noticelost(c);
                }
            }
            game.collectAll();
        };
    };
    game.changeOne = function (c) {
        var ot = c.tag;
        var os = c.sum;
        c.tag = gData.createTag();
        c.sum = 0;
        if (!game.load) {
            game.notice(c, ot, os);
        }
    }
    game.collectOne = function (c) {
        if (c.active) {
            ed = true;
            var s = c.siblings;
            var ot = c.tag, os = c.sum;
            for (var i = 0; i < 3; i++) {
                var si = s[i];
                c.sum += si.sum;
                game.changeOne(si);
            }
            c.sum++;
            if (c.sum > game.sumMax - 1) {
                game.changeOne(c);
            }
            if (!game.load) {
                game.notice(c, ot, os);
            }
            return true;
        }
        return false;
    };
    game.collectAll = function () {
        for (var i = gData.last + 1; i < game.all.length; i++) {
            var c = game.all[i];
            if (c.active) {
                game.noticecollecting(c, c.siblings.map(function (e) { return e.getData(); }));
                gData.last = i;
                game.collectOne(c);
                return i;
            }
        }
        if (gData.last > -1) {
            gData.last = -1;
            return game.collectAll();
        } else {
            game.noticeready();
            delete game.move;
            delete game.stopMoving;
            return -1;
        }
    };
    if (location.href.length == 11) {
        game.load(5, 3);
        game.collectAll(game.rRows[0]);
        game.start2move(game.rRows[0]);
        game.collectOne(game.rRows[0].children[0]);
        game.changeOne(game.rRows[0].children[0]);
    }
    this.game = game;
}).call(window);