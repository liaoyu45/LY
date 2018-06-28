function Him(url, key) {
	"user strict";
	function findForm(a0) {
		if (a0 instanceof HTMLElement) {
			if (!(a0 instanceof HTMLFormElement)) {
				while (!(a0 instanceof HTMLFormElement)) {
					a0 = a0.parentElement;
					if (a0 === document.body) {
						throw "An HTMLFormElement is required outside.";
					}
				}
			}
		}
		return a0;
	}
	var cls = [];
	function makeClass(obj, i, js) {
		var oi = obj[i];
		obj[i] = function (data) {
			for (var i in data || {}) {
				this[i] = data[i];
			}
		};
		cls.push(obj[i]);
		oi.forEach(m=> {
			obj[i].prototype[m.Name] = function () {
				var ev = [...arguments].filter(e=>e instanceof Event)[0] || window.event;
				var a0 = findForm(arguments[0]) || findForm(this[m.Name].RelatedForm);
				var method = a0 instanceof HTMLFormElement,
					data = null,
					u = url;
				if (method) {
					if (!a0.checkValidity()) {
						var s = document.createElement("input");
						s.type = "submit";
						s.style.display = "none";
						a0.appendChild(s);
						s.click();
						s.remove();
						return;
					}
					method = "post";
					data = new FormData(a0);
					data.append(key, m.Key);
					for (var i in this) {
						if (i !== key && typeof this[i] === "object" && !(i in a0)) {
							data.append(i, this[i]);
						}
					}
				} else {
					method = "get";
					u += `?${key}=${m.Key}`;
					var p = {};
					if (m.Parameters) {
						for (var i in this) {
							if (m.Parameters.some(e=>e === i)) {
								p[i] = this[i];
							}
						}
					}
					[...arguments].slice(0, arguments.length - 1).forEach((a, i) => p[m.Parameters[i]] = a);
					for (var i in p) {
						u += `&${i}=${p[i]}`;
					}
				}
				var cb = [...arguments, js[m.Name], function () { }].filter(e=>typeof e === "function")[0];
				var r = new XMLHttpRequest();
				r.open(method, u);
				r.onload = e=> {
					var s = r.responseText;
					if (s.toLowerCase() === "false") {
						s = false;
					} else if (s.toLowerCase() === "true") {
						s = true;
					} else if (s.length && !isNaN(s)) {
						s = (s.indexOf('.') > -1 ? parseFloat : parseInt)(s);
					} else if (new RegExp(/^[{\[].+[}\]]$/).test(s)) {
						try {
							s = JSON.parse(s);
						} catch (e) {
							console.log(e);
						}
					} else if (new RegExp(/^\d+-(1[012]|0?[1-9])-([12][0-9]|0?[1-9])$/).test(s)) {
						s = new Date(Date.parse(s));
					}
					cb.call(this, location.href.length === 11 ? m["Return"] : s);
				};
				r.send(data);
				return r;
			};
		});
	}
	function findClass(obj, js) {
		for (var i in obj) {
			var oi = obj[i];
			var jsi = i in js ? js[i] : {};
			if (oi instanceof Array) {
				makeClass(obj, i, jsi);
			} else {
				findClass(oi, jsi);
			}
		}
	}
	findClass(Him.CSharp, Him.Javascript || {});
	Him.Ready = function () {
		if (location.href.length === 11) {
			cls.forEach(e=> {
				var a = new e();
				for (var i in a) {
					a[i]();
				}
			});
		}
	};
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