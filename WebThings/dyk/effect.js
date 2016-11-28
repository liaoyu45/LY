/// <reference path="/physics.js"/>
/// <reference path="game.js" />
/// <reference path="graphic.js" />
/// <reference path="movement.js" />
game.onnotice = function (hrl) {
    if (game.load) {
        return;
    }
    effect.paint(graphic.getTri(hrl), hrl);
};
game.oncollecting = function (e) {
    if (game.load) {
        return;
    }
    var s = e.siblings;
    var arr;
    var d = e.direction;
    if (d) {
        arr = [[0, 0, 0], [0, 0.5, 1], [0, 1, 0], [1, 0.5, 0]];
    } else {
        arr = [[1, 0, 1], [1, 1, 1], [1, 0.5, 0], [0, 0.5, 1]];
    }
    while (effect.pattern.firstChild) {
        effect.pattern.removeChild(effect.pattern.firstChild);
    }
    arr.forEach(function (ee, i) {
        var tri = graphic.createBasicTri(ee[0], ee[1], ee[2]);
        effect.paint(tri, (s[i] || e).tag, ee[0], (s[i] || e).sum);
        effect.pattern.appendChild(tri);
    });
    var item1 = graphic.getTri(e).points.getItem(1);
    if (!effect.cover) {
        effect.cover = graphic.createElement("svg");
        effect.cover.setAttribute("width", graphic.width * 2);
        effect.cover.setAttribute("height", graphic.height * 2);
    }
    effect.cover.setAttribute("x", item1.x - graphic.width);
    effect.cover.setAttribute("y", d ? item1.y : item1.y - graphic.height * 2);
    graphic.arena.appendChild(effect.cover);
    var border = graphic.createBasicTri(!d, 0, 0, 2);
    border.style.strokeWidth = 4;
    border.style.stroke = "#939";
    border.style.fill = "url(#dyk_clone)";
    border.style.opacity = 1;
    effect.cover.appendChild(border);
    function drawBorder() {
        var c = graphic.width * 6;
        border.style.strokeDasharray = c;
        border.style.strokeDashoffset = c;
        var arr = [];
        for (var i = 0; i < effect.dur; i++) {
            arr.push(c - c / effect.dur * i);
        }
        arr = arr.slice(1);
        arr.push(c);
        var step = 0;
        function r() {
            if (step > arr.length - 1) {
                border.style.strokeDashoffset = 0;
                rotate();
                return;
            }
            border.style.strokeDashoffset = arr[step];
            step++;
            requestAnimationFrame(r);
        }
        r();
    }
    function rotate() {
        var arr = [[], []];
        function addToArr(a) {
            arr[0].push(Math.sin(graphic.ANGLE) / Math.sin(a) * (1 / (Math.sin(a + graphic.ANGLE) / Math.sin(a) + 1)));
            arr[1].push(a);
        }
        for (var i = 0; i < effect.dur; i++) {
            addToArr(i / effect.dur * graphic.ANGLE);
        }
        arr[0] = arr[0].slice(1);
        arr[1] = arr[1].slice(1);
        addToArr(graphic.ANGLE);
        var step = 0;
        var rx = graphic.width;
        var ry = graphic.height * (!d ? 4 : 2) / 3;
        function r() {
            if (step > arr[0].length - 1) {
                while (effect.cover.firstChild) {
                    effect.cover.removeChild(effect.cover.firstChild);
                }
                graphic.arena.removeChild(effect.cover);
                game.collectAll();
                return;
            }
            var s = arr[0][step];
            border.setAttribute("transform", god.formatString(" translate({4} {5}) scale({0}) rotate({1} {2} {3})", s, arr[1][step] * 180 / Math.PI, rx, ry, (1 - s) * rx, (1 - s) * ry));
            step++;
            requestAnimationFrame(r);
        }
        r();
    }
    drawBorder();
};
graphic.onpainting = function (tri, hrl) {
    effect.paint(tri, hrl);
};
movement.onstopped = function () {
    effect.clearMidLayer();
    effect.edges = [];
    effect.front = 0;
    effect.end = 0;
};
movement.onstarted = function () {
    effect.clearMidLayer();
};
movement.onmoving = function (front, end, offset, dataRow, movingRow, cover) {
    if (!offset) {
        return;
    }
    var w = graphic.width;
    if (effect.edges && effect.edges.length) {
        var os = offset;
        while (os < 0) {
            os += w;
        }
        while (os > w) {
            os -= w;
        }
        var s = os / w;
        var m = 4 / 7;
        for (var i = 0; i < effect.edges.length; i++) {
            var t = effect.edges[i];
            var ss = i > 1 ? m + (1 - m) * s : 1 + (m - 1) * s;
            t.e.setAttribute("transform", god.formatString("translate({1} {2}) scale({0})", ss, (1 - ss) * w / 2, (1 - ss) * (t.d ? 1 : 1 / 2) * w / Math.sqrt(3)));
            t.e.style.opacity = i > 1 ? s : 1 - s;
        }
    }
    if (effect.front == front && effect.end == end || offset == 0) {
        return;
    }
    effect.front = front;
    effect.end = end;
    if (effect.hidden) {
        for (var i = 0; i < effect.hidden.length; i++) {
            effect.hidden[i].style.display = "block";
        }
    }
    var f = front + (offset > 0 ? 0 : 2);
    for (var i = 0; i < f; i++) {
        cover.childNodes[i].style.display = "none";
    }
    for (var i = f; i < f - 2 + dataRow.children.length; i++) {
        cover.childNodes[i].style.display = "block";
    }
    for (var i = f - 2 + dataRow.children.length; i < cover.childNodes.length; i++) {
        cover.childNodes[i].style.display = "none";
    }
    var hidden = f > 2 ? [f - 2, f - 1] : [0, 1];
    hidden = [hidden[0], hidden[1], hidden[0] + dataRow.children.length, hidden[1] + dataRow.children.length];
    var edges = [0, 1, dataRow.children.length - 2, dataRow.children.length - 1];
    effect.clearMidLayer();
    effect.edges = [];
    for (var i = 0; i < edges.length; i++) {
        var hrl = dataRow.children[edges[i]];
        if (!hrl) {
            continue;
        }
        var svg = effect.hrlArena(hrl);
        var bg = svg.appendChild(graphic.createBasicTri(hrl.direction));
        bg.style.backgroundColor = graphic.baseColor;
        var t = svg.appendChild(bg.cloneNode());
        effect.paint(t, movingRow[hidden[i]]);
        effect.edges.push({ e: t, d: hrl.direction });
    }
};
movement.onpicking = function (hrl) {
    var svg = effect.hrlArena(hrl);
    var tri = graphic.createBasicTri(hrl.direction);
    svg.appendChild(tri);
    effect.paint(tri, hrl.tag, hrl.direction, hrl.sum);
    var x = graphic.width / 2;
    var y = (hrl.direction ? 2 : 1) * graphic.height / 3;
    var i = 0, a = 0;
    function r() {
        if (i >= effect.dur) {
            effect.clearMidLayer();
            return;
        }
        i += (a += 0.1);
        tri.setAttribute("transform", god.formatString("rotate({0} {1} {2})", i, x, y));
        //tri.style.opacity = (120 - i) / 120;
        requestAnimationFrame(r);
    }
    r();
};
var effect = {};
effect.dur = 22;// god.window.mobile ? 22 : 44;
effect.load = function (settings) {
    var fillarr = [];
    Object.defineProperty(effect, "fillArr", {
        get: function () {
            return fillarr;
        }
    });
    if (settings && settings.tagImages && tagImages.length === game.tagsMax) {
        for (var i = 0; i < game.tagsMax; i++) {
            var img = settings.tagImages[i];
            fillarr.push({ baseColor: img.color, fill: "url(" + img + ")" });
        }
    } else {
        for (var i = 0; i < game.tagsMax; i++) {
            var color = god.random().color();
            fillarr.push({ baseColor: color, fill: color });
        }
    }
    var midLayer = graphic.createElement("svg");
    midLayer.setAttribute("width", graphic.width * game.count);
    midLayer.setAttribute("height", graphic.height * game.count);
    graphic.arena.insertBefore(midLayer, graphic.arena.cover);
    effect.midLayer = midLayer;
    var defs = graphic.arena.appendChild(graphic.createElement("defs"));
    effect.pattern = defs.appendChild(graphic.createElement("pattern"));
    effect.pattern.id = "dyk_clone"
    effect.pattern.setAttribute("width", 1);
    effect.pattern.setAttribute("height", 1);
    for (var i = 0; i < game.sumMax; i++) {
        for (var j = 0; j < game.tagsMax * 2; j++) {
            var pattern = defs.appendChild(graphic.createElement("pattern"));
            pattern.id = god.formatString("dyk_{0}_{1}_{2}", parseInt(j / 2), j % 2, i);
            pattern.setAttribute("width", 1);
            pattern.setAttribute("height", 1);
            var rect = pattern.appendChild(graphic.createElement("rect"));
            rect.setAttribute("width", 111);
            rect.setAttribute("height", 111);
            var fill = fillarr[parseInt(j / 2)];
            fill.id = pattern.id;
            rect.style.fill = fill.fill;
            var content = i ? i.toString() : "";
            var text = pattern.appendChild(graphic.createElement("text"));
            text.setAttribute("stroke", "#fff");
            text.setAttribute("fill", "#fff");
            text.setAttribute("font-size", graphic.height / 3);
            text.setAttribute("x", content.length == 1 ? graphic.width * 3 / 7 : graphic.width * 4 / 11);
            text.setAttribute("y", j % 2 ? graphic.height * 3 / 7 : graphic.height * 5 / 7);
            text.textContent = content;
        }
    }
    game.all.forEach(function (e) {
        effect.paint(graphic.getTri(e), e);
    });
};
effect.paint = function (tri, hrl) {
    var tag, direction, sum;
    if (arguments.length == 2) {
        tag = hrl.tag, direction = hrl.direction ? 0 : 1, sum = hrl.sum;
    } else {
        tag = arguments[1];
        direction = arguments[2] ? 0 : 1;
        sum = arguments[3];
    }
    if (tag < 0 || tag > game.tagsMax) {
        console.log(tag);
    }
    if (direction != 0 && direction != 1) {
        console.log(direction);
    }
    if (sum < 0 || sum > game.sumMax) {
        console.log(sum);
    }
    tri.style.fill = god.formatString("url(#dyk_{0}_{1}_{2})", tag, direction, sum);
};
effect.clearMidLayer = function () {
    while (effect.midLayer.firstChild) {
        effect.midLayer.removeChild(effect.midLayer.firstChild);
    }
};
effect.hrlArena = function (hrl) {
    var tri = graphic.getTri(hrl);
    var i0 = tri.points.getItem(0);
    var i1 = tri.points.getItem(1);
    var svg = effect.midLayer.appendChild(graphic.createElement("svg"));
    svg.style.overflow = "visible";
    svg.setAttribute("width", graphic.width);
    svg.setAttribute("height", graphic.height);
    svg.setAttribute("x", i0.x);
    svg.setAttribute("y", Math.min(i0.y, i1.y));
    return svg;
};
if (god.modes.coding) {
    effect.load();
}