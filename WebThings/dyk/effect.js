/// <reference path="game.js" />
/// <reference path="graphic.js" />
/// <reference path="movement.js" />
var effect = {};
effect.load = function () {
    var defs = graphic.arena.appendChild(graphic.createElement("defs"));
    var cs = [];
    for (var i = 0; i < game.tagsMax; i++) {
        cs.push(god.random().color());
    }
    document.title = cs[0] == cs[1];
    for (var i = 0; i < 12; i++) {
        for (var j = 0; j < game.tagsMax * 2; j++) {
            var pattern = defs.appendChild(graphic.createElement("pattern"));
            pattern.id = god.formatString("dyk_{0}_{1}_{2}", parseInt(j / 2), j % 2, i);
            pattern.setAttribute("width", 1);
            pattern.setAttribute("height", 1);
            var rect = pattern.appendChild(graphic.createElement("rect"));
            rect.setAttribute("width", 111);
            rect.setAttribute("height", 111);
            rect.style.fill = cs[parseInt(j / 2)];
            var text = pattern.appendChild(graphic.createElement("text"));
            text.setAttribute("stroke", "#fff");
            text.setAttribute("fill", "#fff");
            text.setAttribute("font-size", graphic.height / 3);
            text.setAttribute("x", graphic.width * 3 / 7);
            text.setAttribute("y", j % 2 ? graphic.height * 3 / 7 : graphic.height * 5 / 7);
            text.textContent = i * 3 + 1;
        }
    }
};
graphic.onPainting = function (tri, hrl, p) {
    tri.style.fill = god.formatString("url(#dyk_{0}_{1}_{2})", hrl.tag, hrl.direction ? 0 : 1, parseInt(hrl.sum / 3));
    if (p == "tag") {
    }
}