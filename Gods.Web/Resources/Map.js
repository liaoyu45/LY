function AjaxRoute(url, key, obj) {
	"user strict";
	var baseURL = "";
	function makeClass(obj, i) {
		var oi = obj[i];
		var ps = (oi.Properties || []).map(e=> { return { Name: e.Name, Value: null }; });
		obj[i] = function (data) {
			for (var i of ps) {
				i.Value = data[i.Name] || null;
			}
			for (var i in data || {}) {
				if (!ps.some(e=>e.Name === i)) {
					this[i] = data[i];
				}
			}
		};
		ps.forEach(p=>
			Object.defineProperty(n[i].prototype, p.Name, {
				get: () => p.Value,
				set: v=>p.Value = v,
				enumerable: true
			}));
		obj[i].prototype[key] = oi;
		oi.Methods.forEach(m=> {
			obj[i].prototype[m.Name] = function () {
				var method = arguments[0] instanceof HTMLFormElement,
					data = null,
					u = url;
				if (method) {
					method = "post";
					data = new FormData(arguments[0]);
					data.append(key, m.Key);
					for (var i in this) {
						if (i !== key && typeof this[i] === "object" && !(i in arguments[0])) {
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
				var cb = [...arguments].reverse()[0];
				cb = typeof cb === "function" ? cb : this[m.Name][key] || function () { };
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
	function findClass(obj) {
		for (var i in obj) {
			var oi = obj[i];
			if ("Methods" in oi || "Properties" in oi) {
				makeClass(obj, i);
			} else {
				findClass(oi);
			}
		}
	}
	findClass(obj);
	for (var i in obj) {
		window[i] = obj[i];
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