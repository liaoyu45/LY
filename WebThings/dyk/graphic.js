/// <reference path="game.js" />
/// <reference path="base.js" />
var graphic = {};
Object.defineProperties(graphic, {
    ANGLE: {
        value: Math.PI / 3,
        writable: false
    },
    allTris: {
        value: [],
        writable: false
    },
    upPoints: {
        get: function () {
            return [[0, this.height], [this.width / 2, 0], [this.width, this.height]];
        }
    },
    downPoints: {
        get: function () {
            return [[0, 0], [this.width / 2, this.height], [this.width, 0]];
        }
    }
});
graphic.hrl2tri = function (e) {
    var tri = graphic.createBasicTri(e.direction, e.x, e.y);
    god.safe(graphic.onPainting)(tri, e);
    return tri;
};
graphic.createElement = function (name) {
    return document.createElementNS("http://www.w3.org/2000/svg", name);
};
graphic.getElement = function (name) {
    return graphic.arena.getElementsByTagName("http://www.w3.org/2000/svg", name)[0];
};
graphic.createBasicTri = function (d, x, y, s) {
    if (typeof s == "undefined") {
        s = 1;
    }
    x *= graphic.width;
    y *= graphic.height;
    var points = d ? graphic.upPoints : graphic.downPoints;
    for (var i = 0; i < points.length; i++) {
        points[i][0] *= s;
        points[i][1] *= s;
        points[i][0] += x;
        points[i][1] += y;
    }
    return graphic.createPolygon(points);
};
graphic.onPainting = null;
graphic.load = function (w, parent) {
    var h = w * Math.sin(graphic.ANGLE);
    var maxW = w * game.count;
    var maxH = h * game.count;
    var arena = graphic.createElement("svg");
    Object.defineProperty(graphic, "arena", { get: function () { return arena; } });
    arena.setAttribute("width", maxW);
    arena.setAttribute("height", maxH);
    arena.style.overflow = "hidden";
    graphic.createPolygon = function (points) {
        var polygon = graphic.createElement("polygon");
        for (var i = 0; i < points.length; i++) {
            var p = arena.createSVGPoint();
            p.x = points[i][0];
            p.y = points[i][1];
            polygon.points.appendItem(p);
        }
        return polygon;
    };
    var cover = graphic.createPolygon([[0, maxH], [0, 0], [maxW / 2, 0], [0, maxH], [maxW, maxH], [maxW / 2, 0], [maxW, 0], [maxW, maxH], [0, maxH]]);
    cover.style.stroke = "#433";
    cover.style.fill = "#433";
    cover.style.strokeWidth = 0;
    Object.defineProperties(graphic, {
        height: { get: function () { return h; } },
        width: { get: function () { return w; } },
        cover: { get: function () { return cover; } }
    });
    arena.appendChild(cover);
    if (parent) {
        parent.appendChild(arena);
    }
    var allTris = game.all.forEach(function (e) {
        var tri = graphic.hrl2tri(e);
        var ele = { tri: tri, hrl: e };
        graphic.allTris.push(ele);
        arena.appendChild(tri);
    });
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