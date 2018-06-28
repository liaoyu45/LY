var o = {};
var n = "awer";
Object.defineProperty(o, n, (function () {
	var old = [];
	return {
		get: function () {
			var r = function () {
				var e = new Event(n);
				e.target = this;
				for (var i of old) {
					i[1]();
				}
			};
			r.bind(this);
			r.old = old;
			return r;
		},
		set: function (v) {
			if (typeof v === "function") {
				v.bind(this);
				old.push([null, v]);
			}
			if (v instanceof Array) {
				if (v[0] instanceof String && v[1] instanceof Function) {
					old.push([v[0], v[1]]);
				}
			}
		}
	};
})());

var x = new XMLHttpRequest
