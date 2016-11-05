/// <reference path="effect.js" />
/// <reference path="game.js" />
/// <reference path="graphic.js" />
var player = (function () {
    var player = function () {
        var elements = [];
        this.load = function () {
            for (var i = 0; i < game.tagsMax; i++) {
                var arr = [];
                for (var j = 0; j < game.sumMax; j++) {
                    arr.push(j);
                }
                elements.push(arr);
            }
        };
        function addScore(t, s) {
            elements[t][s]++;
        }
        game.onlost = function (c) {
            addScore(c.tag, c.sum);
            god.window.toast(c.sum);
        };
        game.onnotice = function (hrl, ot, os) {
            if (!os) {
                return;
            }
            if (game.move) {
                return;
            }
            god.window.toast(os);
            addScore(ot, os);
        };
    };

    return new player();
})();
