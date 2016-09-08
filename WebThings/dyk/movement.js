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
    if (!game.moving) {
        return;
    }
    if (Math.abs(movement.offset % graphic.width) < graphic.width / 2) {
        movement.record = game.moving(movement.moved / -Math.abs(movement.moved));
    }
    game.stopMoving();
    graphic.arena.removeChild(movement.cover);
};
movement.moving = function (e) {
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
            if (Math.abs(y / x) * 2 / graphic.stretch < graphic.angle) {
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
            offset = y * 2 / graphic.angle / graphic.stretch;
            break;
        case 2:
        default:
            offset = -y * 2 / graphic.angle / graphic.stretch;
            break;
    }
    if (!movement.ongoing) {
        if (Math.abs(offset) < movement.minOffset) {
            return;
        }
        movement.type = t;
        var dataRow = game.allRows[t][movement.hrl[t]];
        game.startMoving(dataRow);
        dataRow.children.forEach(function (e) {
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
            x = y / graphic.angle / graphic.stretch;
            break;
        case 2:
        default:
            s = -y / graphic.height;
            x = -y / graphic.angle / graphic.stretch;
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
    if (!sNow) {
        return;
    }
    var arr = game.moving(sNow);
    if (movement.row.length < arr.length) {
        var inc = s > 0;
        for (var i = 0; i < 2; i++) {
            var ti = inc ? arr.length - 2 + i : 1 - i;
            var hrl = game.nextHRL(movement.getEdge(inc), t, inc);
            hrl.tag = arr[ti];
            movement.cover.appendChild(graphic.hrl2tri(hrl));
            movement.row[inc ? "push" : "unshift"](hrl);
        }
    }
};
movement.startMoving = function (ev) {
    ev.preventDefault();
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
    graphic.arena.appendChild(movement.cover);
    window.addEventListener("blur", movement.stop);
    if (god.window.mobile) {
        movement.startXY = [ev.touches[0].clientX, ev.touches[0].clientY];
        window.addEventListener("touchend", movement.stop);
        window.addEventListener("touchmove", movement.moving);
    } else {
        movement.startXY = [ev.clientX, ev.clientY];
        window.addEventListener("mouseup", movement.stop);
        window.addEventListener("mousemove", movement.moving);
    }
};
movement.pick = function (e) {
    var hrl = graphic.getHRL(e.target);
    hrl.count = 1;
    game.changeTag(hrl);
    game.collect(hrl);
    for (var i = 0; i < 3; i++) {
        game.collect(game.getElement(game.nextHRL(hrl, i)));
    }
};
movement.start = function () {
    var ename = god.window.mobile ? "touchstart" : "mousedown";
    graphic.allTris.forEach(function (e) {
        e.tri.addEventListener(ename, movement.startMoving);
        e.tri.addEventListener("click", movement.pick);
    });
};
movement.pause = function () {
    var ename = god.window.mobile ? "touchstart" : "mousedown";
    graphic.allTris.forEach(function (e) {
        e.tri.removeEventListener(ename, movement.startMoving);
        e.tri.removeEventListener("click", movement.pick);
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
}