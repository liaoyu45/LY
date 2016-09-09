/// <reference path="game.js" />
/// <reference path="base.js" />
var graphic = {};
Object.defineProperties(graphic, {
    angle: {
        value: Math.sqrt(3),
        writable: false
    },
    allTris: {
        value: [],
        writable: false
    }
});
graphic.width = 0;
graphic.height = 0;
game.onNotice = function (hrl) {
    var tri = graphic.getTri(hrl);
    graphic.paint(tri, hrl);
};
graphic.hrl2tri = function (e) {
    var x0 = (game.count - 1 - (e[1] - e[2])) * graphic.width / 2;
    var x1 = x0 + graphic.width / 2;
    var x2 = x0 + graphic.width;
    var y0 = (game.count - e[0]) * graphic.height;
    var y1 = y0 - graphic.height;
    var points = [];
    for (var i = 0; i < 3; i++) {
        points.push(graphic.arena.createSVGPoint());
    }
    if (e.direction) {
        points[0].y = points[2].y = y0;
        points[1].y = y1;
    } else {
        points[0].y = points[2].y = y1;
        points[1].y = y0;
    }
    points[0].x = x0;
    points[1].x = x1;
    points[2].x = x2;
    var tri = graphic.createElement("polygon");
    points.forEach(function (e) { tri.points.appendItem(e); });
    graphic.paint(tri, e, "tag");
    return tri;
};
graphic.createElement = function (name) {
    return document.createElementNS("http://www.w3.org/2000/svg", name);
};
graphic.onPainting = function (tri, hrl, p) {
    if (p == "tag") {
        if (!graphic.onPainting.arr) {
            graphic.onPainting.arr = [];
            for (var i = 0; i < game.tagsMax; i++) {
                graphic.onPainting.arr.push(god.random().color());
            }
        }
        tri.style.fill = graphic.onPainting.arr[hrl.tag];
    }
};
graphic.paint = function (tri, hrl, p) {
    god.safeFunction(graphic.onPainting).execute(tri, hrl, p);
};
graphic.load = function (w, s) {
    var h = w * graphic.angle / 2 * s;
    graphic.width = w;
    graphic.height = h;
    graphic.stretch = s;
    var arena = graphic.createElement("svg");
    graphic.arena = arena;
    var maxW = w * game.count;
    var maxH = h * game.count;
    arena.setAttribute("width", maxW);
    arena.setAttribute("height", maxH);
    arena.style.overflow = "hidden";
    var allTris = game.all.map(function (e) {
        return { tri: graphic.hrl2tri(e), hrl: e };
    });
    allTris.forEach(function (e) {
        graphic.allTris.push(e);
        arena.appendChild(e.tri);
    });
    graphic.arena = arena;
    var cover = graphic.createElement("polygon");
    var points = [[0, maxH], [0, 0], [maxW / 2, 0], [0, maxH], [maxW, maxH], [maxW / 2, 0], [maxW, 0], [maxW, maxH], [0, maxH]];
    points.forEach(function (e) {
        var p = arena.createSVGPoint();
        p.x = e[0];
        p.y = e[1];
        cover.points.appendItem(p);
    });
    cover.style.stroke = "#433";
    cover.style.fill = "#433";
    cover.style.strokeWidth = 5;
    graphic.cover = cover;
    arena.appendChild(cover);
    game.all.forEach(game.collect);
    return arena;
}
graphic.getTri = function (hrl) {
    return graphic.allTris.filter(function (e) {
        return e.hrl[0] == hrl[0] && e.hrl[1] == hrl[1] && e.hrl[2] == hrl[2];
    })[0].tri;
};
graphic.getHRL = function (tri) {
    return graphic.allTris.filter(function (e) {
        return e.tri == tri;
    })[0].hrl;
};
graphic.getRow = function (t, i) {

};
if (location.href.length == 11) {
    game.load(5, 5);
    graphic.load(23, 1);
}
graphic.debugging = false;