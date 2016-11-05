/// <reference path="game.js" />
/// <reference path="base.js" />
var graphic = {};
god.addEventListener(graphic, "painting");
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
    },
    baseColor: {
        writable: true,
        value: "#433"
    }
});
graphic.hrl2tri = function (e) {
    var tri = graphic.createBasicTri(e.direction, e.x, e.y);
    graphic.noticepainting(tri, e);
    return tri;
};
graphic.createElement = function (name) {
    return document.createElementNS("http://www.w3.org/2000/svg", name);
};
graphic.getElement = function (name) {
    return graphic.arena.getElementsByTagName("http://www.w3.org/2000/svg", name)[0];
};
graphic.createBasicTri = function (d, x, y, s) {
    x = god.toDefault(x, 0);
    y = god.toDefault(y, 0);
    s = god.toDefault(s, 1);
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
graphic.load = function (w, parent, settings) {
    if (settings) {
        var arr = [];
        for (var i = 0; i < 3; i++) {
            var s = settings[i];
            if (!s) {
                continue;
            }
            for (var j = game.all.length - 1; j >= 0; j--) {
                if (game.all[j][i] > s) {
                    arr = arr.concat(game.all.splice(j, 1))
                }
            }
        }
        for (var i = 0; i < 3; i++) {
            for (var j = 0; j < arr.length; j++) {
                var c = game.allRows[i][arr[j][i]].children;
                game.allRows[i][arr[j][i]].children.splice(c.indexOf(arr[j]), 1);
            }
        }
    }
    var h = w * Math.sin(graphic.ANGLE);
    var maxW = w * game.count;
    var maxH = h * game.count;
    Object.defineProperties(graphic, {
        height: { get: function () { return h; } },
        width: { get: function () { return w; } },
    });
    if (settings && settings.baseColor) {
        graphic.baseColor = settings.baseColor;
    }
    var arena = graphic.createElement("svg");
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
    cover.style.stroke = graphic.baseColor;
    cover.style.fill = graphic.baseColor;
    cover.style.strokeWidth = 0;
    Object.defineProperties(graphic, {
        cover: { get: function () { return cover; } },
        arena: { get: function () { return arena; } }
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
    god.safe(graphic.arenaCreated)(arena);
    return arena;
};
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
if (location.href.length == 11) {
    game.load(5, 5);
    graphic.load(23, 1);
}