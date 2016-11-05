/// <reference path="game.js" />
/// <reference path="graphic.js" />
/// <reference path="../god.js" />
(function () {
    var mData = { startXY: [0, 0], moved: 0, row: [] };
    Object.defineProperties(mData, {
        dataRow: {
            get: function () {
                return game.allRows[this.type][this.hrl[this.type]];
            }
        },
        direction: {
            get: function () {
                return this.offset > 0;
            }
        }
    });
    mData.getEdge = function (w) {
        if (!mData.row.length) {
            return;
        }
        return mData.row[w ? mData.row.length - 1 : 0];
    };
    mData.stop = function () {
        mData.toggleEvent(mData.stop);
        mData.toggleEvent(mData.stop);
        mData.toggleEvent(mData.move);
    };
    mData.stop.ename = god.window.mobile ? ["blur", "touchend"] : ["blur", "mouseup"];
    mData.stop.window = true;
    mData.getMoveData = function (e) {
        var x, y;
        if (god.window.mobile) {
            x = e.touches[0].clientX - mData.startXY[0];
            y = e.touches[0].clientY - mData.startXY[1];
        } else {
            x = e.clientX - mData.startXY[0];
            y = e.clientY - mData.startXY[1];
        }
        var t;
        if (mData.type > -1) {
            t = mData.type;
        } else {
            if (x * y == 0) {
                if (x == 0) {
                    t = 1;
                } else {
                    t = 0;
                }
            } else {
                if (Math.abs(y / x) < Math.tan(graphic.ANGLE)) {
                    t = 0;
                } else {
                    t = x * y > 0 ? 1 : 2;
                }
            }
        }
        var offset;
        switch (t) {
            case 0:
                offset = -x;
                break;
            case 1:
                offset = y / Math.sin(graphic.ANGLE);
                break;
            case 2:
            default:
                offset = -y / Math.sin(graphic.ANGLE);
                break;
        }
        return { offset: offset, type: t, x: x, y: y };
    };
    mData.final = function () {
        mData.toggleEvent(mData.final);
        mData.toggleEvent(mData.move);
        if (Math.abs(mData.offset % graphic.width) < graphic.width / 2) {
            mData.record = game.move(mData.moved / -Math.abs(mData.moved));
        }
        game.stopMoving();
        mData.offset = 0;
        graphic.arena.removeChild(mData.cover);
        movement.noticestopped();
    };
    mData.final.ename = god.window.mobile ? ["touchend", "blur"] : ["mouseup", "blur"];
    mData.final.window = true;
    mData.restart = function () {
        mData.toggleEvent(mData.restart);
        mData.toggleEvent(mData.move);
        mData.start();
    };
    mData.restart.ename = god.window.mobile ? "touchend" : "mouseup";
    mData.restart.window = true;
    mData.move = function (e) {
        if (mData.pick.ing) {
            mData.pick.ing = false;
            mData.toggleEvent(mData.pick);
            mData.toggleEvent(mData.restart, true);
        }
        var md = mData.getMoveData(e);
        if (mData.type == -1) {
            if (Math.abs(md.offset) < graphic.width / 6 || mData.hrl[md.type] == game.count - 1) {
                return;
            }
            mData.toggleEvent(mData.final, true);
            mData.toggleEvent(mData.final, true);
            mData.toggleEvent(mData.restart);
            movement.noticestarted();
            mData.type = md.type;
            graphic.arena.appendChild(mData.cover);
            game.start2move(mData.dataRow);
            mData.dataRow.children.forEach(function (e) {
                var ele = graphic.hrl2tri(e);
                mData.row.push(e);
                mData.cover.appendChild(ele);
            });
            graphic.arena.appendChild(graphic.cover);
        }
        var s;
        switch (mData.type) {
            case 0:
                s = -md.x / graphic.width;
                md.y = 0;
                break;
            case 1:
                s = md.y / graphic.height;
                md.x = md.y / Math.tan(graphic.ANGLE);
                break;
            case 2:
            default:
                s = -md.y / graphic.height;
                md.x = -md.y / Math.tan(graphic.ANGLE);
                break;
        }
        if (s > 0) {
            s = Math.ceil(s);
        } else {
            s = Math.floor(s);
        }
        var overDrag;
        if (s > mData.maxMovedPlus) {
            if (s - mData.maxMovedPlus > 1) {
                overDrag = true;
            } else {
                mData.maxMovedPlus = s;
            }
        } else if (s < mData.maxMovedMinus) {
            if (s - mData.maxMovedMinus < -1) {
                overDrag = true;
            } else {
                mData.maxMovedMinus = s;
            }
        }
        if (overDrag) {
            mData.stop();
            return;
        }
        mData.offset = md.offset;
        mData.cover.setAttribute("x", md.x);
        if (md.y) {
            mData.cover.setAttribute("y", md.y);
        }
        var sNow = s - mData.moved;
        mData.moved = s;
        var shaker = game.move(sNow);
        var arr = shaker.arr;
        var inc = s > 0;
        if (mData.row.length < arr.length) {
            var next = (mData.type + 1) % 3;
            for (var i = 0; i < 2; i++) {
                var ti = inc ? arr.length - 2 + i : 1 - i;
                var edge = mData.getEdge(inc);
                var inT = edge.fullSiblings.filter(function (ee) { return ee[mData.type] == edge[mData.type]; });
                var cpr = inT[0][next] > inT[1][next];
                var hrl = (inc ^ cpr) ? inT[0] : inT[1];
                hrl.tag = arr[ti];
                var tri = graphic.hrl2tri(hrl);
                if (inc) {
                    mData.cover.appendChild(tri);
                } else {
                    mData.cover.insertBefore(tri, mData.cover.firstChild);
                }
                mData.row[inc ? "push" : "unshift"](hrl);
            }
        }
        movement.noticemoving(shaker.front, shaker.end, mData.offset, mData.dataRow, mData.row, mData.cover);
    };
    mData.move.ename = god.window.mobile ? "touchmove" : "mousemove";
    mData.move.window = true;
    mData.blur = function () {
        mData.pick.ing = false;
        mData.toggleEvent(mData.pick);
        mData.toggleEvent(mData.move);
        mData.toggleEvent(mData.blur);
    };
    mData.blur.ename = "blur";
    mData.blur.window = true;
    mData.start2move = function (ev) {
        ev.preventDefault();
        mData.toggleEvent(mData.start2move);
        mData.pick.ing = true;
        mData.toggleEvent(mData.pick, true);
        mData.toggleEvent(mData.move, true);
        mData.toggleEvent(mData.blur, true);
        mData.moved = 0;
        mData.row = [];
        mData.type = -1;
        mData.maxMovedPlus = mData.maxMovedMinus = 0;
        mData.hrl = graphic.getHRL(ev.target);
        while (mData.cover.firstChild) {
            mData.cover.removeChild(mData.cover.firstChild);
        }
        mData.cover.setAttribute("x", 0);
        mData.cover.setAttribute("y", 0);
        if (god.window.mobile) {
            mData.startXY = [ev.touches[0].clientX, ev.touches[0].clientY];
        } else {
            mData.startXY = [ev.clientX, ev.clientY];
        }
    };
    mData.start2move.ename = god.window.mobile ? "touchstart" : "mousedown";
    mData.pick = function (e) {
        mData.blur();
        movement.noticepicking(mData.hrl);
        game.changeOne(mData.hrl);
        game.collectAll();
    };
    mData.pick.ename = god.window.mobile ? "touchend" : "mouseup";
    mData.start = function () {
        mData.toggleEvent(mData.start2move, true);
    };
    mData.start.ename = god.window.mobile ? "touchstart" : "mousedown";
    mData.toggleEvent = function (func, state) {
        var ev = mData.allEvents.filter(function (e) {
            return e.func == func;
        })[0];
        var action = (state ? "add" : "remove") + "EventListener";
        var n = ev.ename;
        function r(n) {
            if (ev.window) {
                window[action](n, ev.func);
            } else {
                graphic.allTris.forEach(function (e) {
                    e.tri[action](n, ev.func);
                });
            }
        }
        if (typeof n == "string") {
            r(n);
        } else {
            for (var i = 0; i < n.length; i++) {
                r(n[i]);
            }
        }
    };
    mData.allEvents = [];
    for (var i in mData) {
        var f = mData[i];
        var ename = f.ename;
        if (!ename) {
            continue;
        }
        mData.allEvents.push({ ename: ename, func: f, window: f.window });
    }
    var movement = {};
    god.addEventListener(movement, "picking", "picked", "stopped", "started", "moving");
    movement.load = function () {
        game.onready = mData.start;
        var cover = graphic.arena.appendChild(graphic.arena.cloneNode());
        cover.style.overflow = "visible";
        mData.cover = cover;
        mData.start();
        delete movement.load;
    };

    if (location.href.length == 11) {
        mData.load();
        mData.start();
        mData.move();
    }
    this.movement = movement;
}).call(window);