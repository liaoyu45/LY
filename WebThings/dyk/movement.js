/// <reference path="base.js" />
/// <reference path="graphic.js" />
/// <reference path="../god.js" />
var movement = { startXY: [0, 0], moved: 0, row: [] };
Object.defineProperties(movement, {
    minOffset: {
        get: function () {
            return graphic.width / 6;
        }
    },
    ongoing: {
        get: function () {
            return this.type > -1;
        }
    },
    dataRow: {
        get: function () {
            return game.allRows[this.type][this.hrl[this.type]];
        }
    }
});
movement.getEdge = function (w) {
    if (!movement.row.length) {
        return;
    }
    return movement.row[w ? movement.row.length - 1 : 0];
};
movement.stop = function () {
    window.removeEventListener("blur", movement.stop);
    if (god.window.mobile) {
        window.removeEventListener("touchend", movement.stop);
        window.removeEventListener("touchmove", movement.moving);
    } else {
        window.removeEventListener("mouseup", movement.stop);
        window.removeEventListener("mousemove", movement.moving);
    }
    if (!game.stopMoving) {
        return;
    }
    if (Math.abs(movement.offset % graphic.width) < graphic.width / 2) {
        movement.record = game.moving(movement.moved / -Math.abs(movement.moved));
    }
    game.stopMoving();
    graphic.arena.removeChild(movement.cover);
    god.safe(movement.onStopped)();
};
movement.moving = function (e) {
    movement.clicking = false;
    var x, y;
    if (god.window.mobile) {
        x = e.touches[0].clientX - movement.startXY[0];
        y = e.touches[0].clientY - movement.startXY[1];
    } else {
        x = e.clientX - movement.startXY[0];
        y = e.clientY - movement.startXY[1];
    }
    var t;
    if (movement.ongoing) {
        t = movement.type;
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
    if (!movement.ongoing) {
        if (Math.abs(offset) < movement.minOffset) {
            return;
        }
        if (movement.hrl[t] == game.count - 1) {
            return;
        }
        god.safe(movement.onStarted)();
        movement.type = t;
        graphic.arena.appendChild(movement.cover);
        game.startMoving(movement.dataRow);
        movement.dataRow.children.forEach(function (e) {
            var ele = graphic.hrl2tri(e);
            movement.row.push(e);
            movement.cover.appendChild(ele);
        });
        graphic.arena.appendChild(graphic.cover);
    }
    movement.offset = offset;
    var s;
    switch (t) {
        case 0:
            s = -x / graphic.width;
            y = 0;
            break;
        case 1:
            s = y / graphic.height;
            x = y / Math.tan(graphic.ANGLE);
            break;
        case 2:
        default:
            s = -y / graphic.height;
            x = -y / Math.tan(graphic.ANGLE);
            break;
    }
    movement.cover.setAttribute("x", x);
    if (y) {
        movement.cover.setAttribute("y", y);
    }
    if (s > 0) {
        s = Math.ceil(s);
    } else {
        s = Math.floor(s);
    }
    var overDrag;
    if (s > movement.maxMovedPlus) {
        if (s - movement.maxMovedPlus > 1) {
            overDrag = true;
        } else {
            movement.maxMovedPlus = s;
        }
    } else if (s < movement.maxMovedMinus) {
        if (s - movement.maxMovedMinus < -1) {
            overDrag = true;
        } else {
            movement.maxMovedMinus = s;
        }
    }
    if (overDrag) {
        movement.stop();
        return;
    }
    var sNow = s - movement.moved;
    movement.moved = s;
    var shaker = game.moving(sNow);
    var arr = shaker.arr;
    var inc = s > 0;
    if (movement.row.length < arr.length) {
        var next = (t + 1) % 3;
        for (var i = 0; i < 2; i++) {
            var ti = inc ? arr.length - 2 + i : 1 - i;
            var edge = movement.getEdge(inc);
            var inT = edge.fullSiblings.filter(function (ee) { return ee[t] == edge[t]; });
            var cpr = inT[0][next] > inT[1][next];
            var hrl = (inc ^ cpr) ? inT[0] : inT[1];
            hrl.tag = arr[ti];
            var tri = graphic.hrl2tri(hrl);
            if (inc) {
                movement.cover.appendChild(tri);
            } else {
                movement.cover.insertBefore(tri, movement.cover.firstChild);
            }
            movement.row[inc ? "push" : "unshift"](hrl);
        }
    }
    god.safe(movement.onMoving)(shaker.front, shaker.end);
};
movement.startMoving = function (ev) {
    ev.preventDefault();
    if (game.moving) {
        return;
    }
    movement.clicking = true;
    movement.moved = 0;
    movement.row = [];
    movement.type = -1;
    movement.maxMovedPlus = movement.maxMovedMinus = 0;
    movement.hrl = graphic.allTris.filter(function (el) {
        return ev.target == el.tri;
    })[0].hrl;
    while (movement.cover.firstChild) {
        movement.cover.removeChild(movement.cover.firstChild);
    }
    movement.cover.setAttribute("x", 0);
    movement.cover.setAttribute("y", 0);
    window.addEventListener("blur", movement.stop);
    var stop, moving;
    if (god.window.mobile) {
        movement.startXY = [ev.touches[0].clientX, ev.touches[0].clientY];
        stop = "touchend";
        moving = "touchmove"
    } else {
        movement.startXY = [ev.clientX, ev.clientY];
        stop = "mouseup";
        moving = "mousemove"
    }
    window.addEventListener(stop, movement.stop);
    window.addEventListener(moving, movement.moving);
};
movement.pick = function (e) {
    if (!movement.clicking) {
        return;
    }
    var hrl = graphic.getHRL(e.target);
    game.changeOne(hrl);
    game.collectAll();
};
movement.start = function () {
    var start = god.window.mobile ? "touchstart" : "mousedown";
    var end = god.window.mobile ? "touchend" : "mouseup";
    graphic.allTris.forEach(function (e) {
        e.tri.addEventListener(start, movement.startMoving);
        e.tri.addEventListener(end, movement.pick);
    });
};
movement.pause = function () {
    var start = god.window.mobile ? "touchstart" : "mousedown";
    var end = god.window.mobile ? "touchend" : "mouseup";
    graphic.allTris.forEach(function (e) {
        e.tri.removeEventListener(start, movement.startMoving);
        e.tri.removeEventListener(end, movement.pick);
    });
};
movement.load = function () {
    if (god.window.mobile) {
        graphic.arena.addEventListener("touchstart", function (e) {
            e.preventDefault();
        });
    }
    var cover = graphic.arena.appendChild(graphic.arena.cloneNode());
    cover.style.overflow = "visible";
    movement.cover = cover;
};

if (location.href.length == 11) {
    movement.load();
    movement.start();
    movement.moving();
}