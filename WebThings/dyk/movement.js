/// <reference path="base.js" />
/// <reference path="graphic.js" />
var movement = { startXY: [0, 0], ongoing: false, started: false, moved: 0, row: {}, first: {}, last: {} };
Object.defineProperties(movement, {
    minOffset: {
        get: function () {
            return graphic.width / 6;
        }
    },
});
movement.stop = function () {
    window.removeEventListener("mouseup", movement.stop);
    window.removeEventListener("blur", movement.stop);
    window.removeEventListener("mousemove", movement.moving);

    window.removeEventListener("touchend", movement.stop);
    window.removeEventListener("touchmove", movement.moving);
    if (!game.moving) {
        return;
    }
    if (Math.abs(movement.offset % graphic.width) < graphic.width / 2) {
        game.moving(movement.moved / -Math.abs(movement.moved));
    }
    game.stopMoving();
    graphic.arena.removeChild(movement.cover);
    movement.started = movement.ongoing = false;
    movement.moved = 0;
};
movement.moving = function (e) {
    if (!movement.started) {
        return;
    }
    var x, y;
    if (e.clientX) {
        x = e.clientX - movement.startXY[0];
        y = e.clientY - movement.startXY[1];
    } else {
        x = e.touches[0].clientX - movement.startXY[0];
        y = e.touches[0].clientY - movement.startXY[1];
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
        movement.ongoing = true;
        movement.type = t;
        var row = game.startMoving(t, movement.hrl[t]);
        row.children.forEach(function (e) {
            movement.cover.appendChild(graphic.hrl2tri(e).tri);
        });
        movement.first = row.children[0];
        movement.last = row.children[row.children.length - 1];
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
    movement.cover.setAttribute("y", y);
    if (s > 0) {
        s = Math.ceil(s);
    } else {
        s = Math.floor(s);
    }
    var sNow = s - movement.moved;
    movement.moved = s;
    var record = game.moving(sNow);
    if (movement.cover.childNodes.length < record.arr.length) {
        var inc = s > 0;
        var hrl = inc ? movement.last : movement.first;
        var indexes = inc ? [record.arr.length - 2, record.arr.length - 1] : [1, 0];
        for (var i = 0; i < 2; i++) {
            hrl = game.nextHRL(hrl, t, inc);
            movement.cover.appendChild(graphic.hrl2tri(hrl).tri).style.fill = graphic.fillArr[record.arr[indexes[i]]];
        }
        if (inc) {
            movement.last = hrl;
        } else {
            movement.first = hrl;
        }
    }
    movement.record = record;
};
game.onTagChanged = function (e, n, s) {
    var ele = graphic.getTri(e);
    ele.tri.style.fill = graphic.fillArr[n];
};
movement.start = function () {
    graphic.allTris.forEach(function (e) {
        e.tri.onmousedown = e.tri.ontouchstart = function (ev) {
            movement.started = true;
            if (ev.clientX) {
                movement.startXY = [ev.clientX, ev.clientY];
            } else {
                movement.startXY = [ev.touches[0].clientX, ev.touches[0].clientY];
            }
            movement.hrl = graphic.allTris.filter(function (el) {
                return ev.target == el.tri;
            })[0].hrl;
            while (movement.cover.firstChild) {
                movement.cover.removeChild(movement.cover.firstChild);
            }
            graphic.arena.appendChild(movement.cover);
            window.addEventListener("mouseup", movement.stop);
            window.addEventListener("blur", movement.stop);
            window.addEventListener("mousemove", movement.moving);

            window.addEventListener("touchend", movement.stop);
            window.addEventListener("touchmove", movement.moving);
        };
    });
    var cover = graphic.arena.appendChild(graphic.arena.cloneNode());
    cover.style.overflow = "visible";
    movement.cover = cover;
};

if (location.href.length == 11) {
    movement.start();
}