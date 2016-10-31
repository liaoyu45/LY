/// <reference path="base.js" />
/// <reference path="graphic.js" />
/// <reference path="../god.js" />
var movement = { startXY: [0, 0], moved: 0, row: [] };
god.addEventListener(movement, "picking", "picked", "stopped", "started", "moving");
Object.defineProperties(movement, {
    minOffset: {
        get: function () {
            return graphic.width / 6;
        }
    },
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
movement.getEdge = function (w) {
    if (!movement.row.length) {
        return;
    }
    return movement.row[w ? movement.row.length - 1 : 0];
};
movement.stop = function () {
    var end = god.window.mobile ? "touchend" : "mouseup";
    var moving = god.window.mobile ? "touchmove" : "mousemove";
    window.removeEventListener("blur", movement.stop);
    window.removeEventListener(end, movement.stop);
    window.removeEventListener(moving, movement.moving);
    if (!game.stopMoving) {
        movement.start();
        return;
    }
    if (Math.abs(movement.offset % graphic.width) < graphic.width / 2) {
        movement.record = game.move(movement.moved / -Math.abs(movement.moved));
    }
    game.stopMoving();
    movement.offset = 0;
    graphic.arena.removeChild(movement.cover);
    movement.noticestopped();
};
movement.getMoveData = function (e) {
    var x, y;
    if (god.window.mobile) {
        x = e.touches[0].clientX - movement.startXY[0];
        y = e.touches[0].clientY - movement.startXY[1];
    } else {
        x = e.clientX - movement.startXY[0];
        y = e.clientY - movement.startXY[1];
    }
    var t;
    if (movement.type > -1) {
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
    return { offset: offset, type: t, x: x, y: y };
};
movement.moving = function (e) {
    if (movement.pick.ing) {
        movement.pick.ing = false;
        var pick = god.window.mobile ? "touchend" : "mouseup";
        graphic.allTris.forEach(function (e) {
            e.tri.removeEventListener(pick, movement.pick);
        });
    }
    var md = movement.getMoveData(e);
    if (movement.type == -1) {
        if (Math.abs(md.offset) < movement.minOffset || movement.hrl[md.type] == game.count - 1) {
            return;
        }
        movement.noticestarted();
        movement.type = md.type;
        graphic.arena.appendChild(movement.cover);
        game.start2move(movement.dataRow);
        movement.dataRow.children.forEach(function (e) {
            var ele = graphic.hrl2tri(e);
            movement.row.push(e);
            movement.cover.appendChild(ele);
        });
        graphic.arena.appendChild(graphic.cover);
    }
    var s;
    switch (movement.type) {
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
    movement.offset = md.offset;
    movement.cover.setAttribute("x", md.x);
    if (md.y) {
        movement.cover.setAttribute("y", md.y);
    }
    var sNow = s - movement.moved;
    movement.moved = s;
    var shaker = game.move(sNow);
    var arr = shaker.arr;
    var inc = s > 0;
    if (movement.row.length < arr.length) {
        var next = (movement.type + 1) % 3;
        for (var i = 0; i < 2; i++) {
            var ti = inc ? arr.length - 2 + i : 1 - i;
            var edge = movement.getEdge(inc);
            var inT = edge.fullSiblings.filter(function (ee) { return ee[movement.type] == edge[movement.type]; });
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
    movement.noticemoving(shaker.front, shaker.end, movement.offset);
};
movement.start2move = function (ev) {
    ev.preventDefault();
    var start = god.window.mobile ? "touchstart" : "mousedown";
    graphic.allTris.forEach(function (e) {
        e.tri.removeEventListener(start, movement.start2move);
    });
    movement.pick.ing = true;
    var pick = god.window.mobile ? "touchend" : "mouseup";
    graphic.allTris.forEach(function (e) {
        e.tri.addEventListener(pick, movement.pick);
    });
    var stop = god.window.mobile ? "touchend" : "mouseup";
    var moving = god.window.mobile ? "touchmove" : "mousemove";
    window.addEventListener(stop, movement.stop);
    window.addEventListener(moving, movement.moving);
    window.addEventListener("blur", movement.stop);
    ev.preventDefault();
    if (game.locked) {
        return;
    }
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
    if (god.window.mobile) {
        movement.startXY = [ev.touches[0].clientX, ev.touches[0].clientY];
    } else {
        movement.startXY = [ev.clientX, ev.clientY];
    }
};
movement.pick = function (e) {
    var end = god.window.mobile ? "touchend" : "mouseup";
    graphic.allTris.forEach(function (e) {
        e.tri.removeEventListener(end, movement.pick);
    });
    movement.stop();
    movement.noticepicking(movement.hrl);
    game.changeOne(movement.hrl);
    game.collectAll();
};
movement.start = function () {
    var start = god.window.mobile ? "touchstart" : "mousedown";
    graphic.allTris.forEach(function (e) {
        e.tri.addEventListener(start, movement.start2move);
    });
};
movement.load = function () {
    game.onready = movement.start;
    if (god.window.mobile) {
        graphic.arena.addEventListener("touchstart", function (e) {
            e.preventDefault();
        });
    }
    var cover = graphic.arena.appendChild(graphic.arena.cloneNode());
    cover.style.overflow = "visible";
    movement.cover = cover;
    movement.start();
};

if (location.href.length == 11) {
    movement.load();
    movement.start();
    movement.moving();
}