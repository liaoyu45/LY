/// <reference path="/physics.js"/>
/// <reference path="game.js" />
/// <reference path="graphic.js" />
/// <reference path="movement.js" />
var effect = {};
effect.fillArr = [];
effect.load = function (s) {
    effect.defs = graphic.arena.appendChild(graphic.createElement("defs"));
    effect.pattern = effect.defs.appendChild(graphic.createElement("pattern"));
    effect.pattern.id = "dyk_clone"
    effect.pattern.setAttribute("width", 1);
    effect.pattern.setAttribute("height", 1);
    for (var i = 0; i < game.tagsMax; i++) {
        var color = god.random().color();
        effect.fillArr.push({ baseColor: color, image: color });
    }
    for (var i = 0; i < game.sumMax; i++) {
        for (var j = 0; j < game.tagsMax * 2; j++) {
            var pattern = effect.defs.appendChild(graphic.createElement("pattern"));
            pattern.id = god.formatString("dyk_{0}_{1}_{2}", parseInt(j / 2), j % 2, i);
            pattern.setAttribute("width", 1);
            pattern.setAttribute("height", 1);
            var rect = pattern.appendChild(graphic.createElement("rect"));
            rect.setAttribute("width", 111);
            rect.setAttribute("height", 111);
            var fill = effect.fillArr[parseInt(j / 2)];
            fill.id = pattern.id;
            rect.style.fill = fill.image;
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

    effect.rotate = function (x, y, d) {

    };
    game.all.forEach(function (e) {
        effect.paint(graphic.getTri(e), e);
    });
};
graphic.onPainting = function (tri, hrl) {
    effect.paint(tri, hrl);
};
game.onNotice = function (hrl) {
    if (game.load) {
        return;
    }
    effect.paint(graphic.getTri(hrl), hrl);
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
game.onCollecting = function (e) {
    if (game.load) {
        return;
    }
    var s = e.siblings;
    var arr;
    var d = e.direction;
    if (d) {
        arr = [[0, 0, 0], [0, 0.5, 1], [0, 1, 0], [1, 0.5, 0]];
    } else {
        arr = [[1, 0.5, 0], [1, 1, 1], [1, 0, 1], [0, 0.5, 1]];
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
        var dur = 44;
        for (var i = 0; i < dur; i++) {
            arr.push(laws.tween.cubic.easeInOut(i, c, -c, dur));
        }
        arr = arr.slice(1);
        arr.push(c);
        var step = 0;
        function r() {
            if (step > arr.length - 1) {
                rotate();
                return;
            }
            border.style.strokeDashoffset = arr[step];
            step++;
            requestAnimationFrame(r);
        }
        r();
    }
    drawBorder();
    function rotate() {
        var dur = 44;
        var arr = [[], []];
        function addToArr(a) {
            arr[0].push(Math.sin(graphic.ANGLE) / Math.sin(a) * (1 / (Math.sin(a + graphic.ANGLE) / Math.sin(a) + 1)));
            arr[1].push(a);
        }
        for (var i = 0; i < dur; i++) {
            addToArr(i / dur * graphic.ANGLE);
        }
        arr[0] = arr[0].slice(1);
        arr[1] = arr[1].slice(1);
        addToArr(graphic.ANGLE);
        var step = 0;
        var rx = graphic.width;
        var ry = graphic.height * (!d ? 4 : 2) / 3;
        function r() {
            if (step > arr[0].length - 1) {
                graphic.arena.removeChild(effect.cover);
                game.collectAll();
                return;
            }
            border.style.opacity = parseFloat(border.style.opacity) - 1 / dur;
            var s = arr[0][step];
            border.setAttribute("transform", god.formatString(" translate({4} {5}) scale({0}) rotate({1} {2} {3})", s, arr[1][step] * 180 / Math.PI, rx, ry, (1 - s) * rx, (1 - s) * ry));
            step++;
            requestAnimationFrame(r);
        }
        r();
    }
};
if (god.modes.coding) {
    effect.load();
}
function centerRS(data, image) {
}
