/// <reference path="base.js" />
var graphic = {};
Object.defineProperties(graphic, {
    "namespaceURI": {
        value: "http://www.w3.org/2000/svg",
        writable: false
    },
    angle:{
        value:Math.sqrt(3),
        writable: false
    },
    allTris: {
        value: [],
        writable: false
    },
    fillArr: {
        value: [],
        writable: false
    }
});
graphic.width = 0;
graphic.height = 0;
graphic.hrl2tri = function (e) {
    var x0 = (game.count - 1 - (e.right - e.left)) * graphic.width / 2;
    var x1 = x0 + graphic.width / 2;
    var x2 = x0 + graphic.width;
    var y0 = (game.count - e.horizon) * graphic.height;
    var y1 = y0 - graphic.height;
    var up = game.count - e.horizon - e.left - e.right == 1;
    var points = [];
    for (var i = 0; i < 3; i++) {
        points.push(graphic.arena.createSVGPoint());
    }
    if (up) {
        points[0].y = points[2].y = y0;
        points[1].y = y1;
    } else {
        points[0].y = points[2].y = y1;
        points[1].y = y0;
    }
    points[0].x = x0;
    points[1].x = x1;
    points[2].x = x2;
    var tri = document.createElementNS(graphic.namespaceURI, "polygon");
    points.forEach(function (e) { tri.points.appendItem(e); });
    tri.style.fill = graphic.fillArr[e.tag];
    return { hrl: e, tri: tri };
};
graphic.load = function (w, s) {
    var getRandomColor = function () {
        return "#" + ("00000" + ((Math.random() * 16777215 + 0.5) >> 0).toString(16)).slice(-6);
    }
    for (var i = 0; i < game.tagsMax; i++) {
        graphic.fillArr.push(getRandomColor());
    }
    var h = w * Math.sqrt(3) / 2 * s;
    graphic.width = w;
    graphic.height = h;
    graphic.stretch = s;
    var arena = document.createElementNS(graphic.namespaceURI, "svg");
    graphic.arena = arena;
    var maxW = w * game.count;
    var maxH = h * game.count;
    arena.setAttribute("width", maxW);
    arena.setAttribute("height", maxH);
    arena.style.overflow = "hidden";
    var allTris = game.all.map(graphic.hrl2tri);
    allTris.forEach(function (e) {
        graphic.allTris.push(e);
        arena.appendChild(e.tri);
    });
    graphic.arena = arena;
    var cover = document.createElementNS(graphic.namespaceURI, "polygon");
    var points = [[0, maxH], [0, 0], [maxW / 2, 0], [0, maxH], [maxW, maxH], [maxW / 2, 0], [maxW, 0], [maxW, maxH], [0, maxH]];
    points.forEach(function (e) {
        var p = arena.createSVGPoint();
        p.x = e[0];
        p.y = e[1];
        cover.points.appendItem(p);
    });
    graphic.cover = cover;
    cover.style.stroke = "#9f9";
    cover.style.strokeWidth = 5;
    arena.appendChild(cover);
    return arena;
}
graphic.getTri = function (hrl) {
    return graphic.allTris.filter(function (e) {
        return e.hrl.horizon == hrl[0] && e.hrl.right == hrl[1] && e.hrl.left == hrl[2];
    })[0];
};
graphic.getRow = function (t, i) {

};
if (location.href.length == 11) {
    game.load(5, 5);
    graphic.load(23, 1);
}