function Him(url, key) {
	"user strict";
	function makeClass(obj, i, js) {
		var oi = obj[i];
		obj[i] = function (data) {
			for (var i in data || {}) {
				this[i] = data[i];
			}
		};
		oi.Methods.forEach(m=> {
			if (location.href.length === 11) {
				js[m.Name](m["Return"]);
			}
			obj[i].prototype[m.Name] = function () {
				var method = arguments[0] instanceof HTMLFormElement,
					data = null,
					u = url;
				if (method) {
					var f = arguments[0];
					if (!f.checkValidity()) {
						var s = document.createElement("input");
						s.type = "submit";
						s.style.display = "none";
						f.appendChild(s);
						s.click();
						s.remove();
						return;
					}
					method = "post";
					data = new FormData(f);
					data.append(key, m.Key);
					for (var i in this) {
						if (i !== key && typeof this[i] === "object" && !(i in f)) {
							data.append(i, this[i]);
						}
					}
				} else {
					method = "get";
					u += `?${key}=${m.Key}`;
					var p = {};
					for (var i in this) {
						if (m.Parameters.some(e=>e === i)) {
							p[i] = this[i];
						}
					}
					[...arguments].slice(0, arguments.length - 1).forEach((a, i) => p[m.Parameters[i]] = a);
					for (var i in p) {
						u += `&${i}=${p[i]}`;
					}
				}
				var cb = [[...arguments].reverse()[0], js[m.Name], function () { }].filter(e=>typeof e === "function")[0];
				if (location.href.length === 11) {
					cb(m["Return"]);
				}
				var r = new XMLHttpRequest();
				r.open(method, u);
				r.onload = e=> {
					if (e.currentTarget.responseText) {
						cb(JSON.parse(e.currentTarget.responseText));
					}
				};
				r.send(data);
				return r;
			};
		});
	}
	function findClass(obj, js) {
		for (var i in obj) {
			var oi = obj[i];
			if ("Methods" in oi || "Properties" in oi) {
				makeClass(obj, i, js[i]);
			} else {
				findClass(oi, js[i]);
			}
		}
	}
	findClass(Him.CSharp, Him.Javascript);
	for (var i in Him.CSharp) {
		window[i] = Him.CSharp[i];
	}
}
addEventListener("load", function () {
	[...document.querySelectorAll("input[type=button]")].filter(e=>e.dataset.god).forEach(e=> {
		e.onclick = () => {
			var form = e.parentElement;
			while (!(form instanceof HTMLFormElement)) {
				form = form.parentElement;
			}
			var t = window,
				ev = false;
			[...e.dataset.god.split('.')].forEach(e=> {
				if (typeof t[e] === "function") {
					if (ev) {
						t[e](form);
					} else {
						ev = t = new t[e]();
					}
				} else {
					t = t[e];
				}
			});
		};
	});
});