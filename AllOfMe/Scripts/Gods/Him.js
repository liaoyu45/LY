function Him(url, key) {
	"user strict";
	var coding = location.href.length === 11;
	function isValid(e) {
		return e instanceof String || !isNaN(e) || e instanceof Date || e instanceof Boolean;
	}
	var cls = [];
	function makeClass(obj, n, jo) {
		var oi = obj[n];
		var obo = Symbol();//one by one
		var ing = Symbol();
		obj[n] = function (data) {
			if (data instanceof Boolean) {
				this[obo] = data;
				data = arguments[1];
			} else {
				this[obo] = true;
			}
			for (var i in data || {}) {
				this[i] = data[i];
			}
		};
		cls.push(obj[n]);
		oi.forEach(m=> {
			obj[n].prototype[m.Name] = function () {
				if (this[obo]) {
					if (this[ing]) {
						return;
					}
					this[ing] = true;
				}
				var ev = [...arguments].filter(e=>e instanceof Event)[0] || window.event;
				var a0 = arguments[0];
				if (a0 instanceof HTMLElement) {//while coding, it is true, why?
					while (!(a0 instanceof HTMLFormElement)) {
						a0 = a0.parentElement;
						if (a0 === document.body) {
							throw "";
						}
					}
				}
				var method = a0 instanceof HTMLFormElement,
					data = null,
					u = url;
				if (method && !coding) {
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
						if (i.constructor !== Symbol && isValid(this[i]) && !(i in a0)) {
							data.append(i, this[i]);
						}
					}
				} else {
					method = "get";
					u += `?${key}=${m.Key}`;
					if (m.Parameters) {
						var p = {};
						for (var i in this) {
							if (m.Parameters.some(e=>e === i)) {
								p[i] = this[i];
							}
						}
						if (a0 instanceof Object) {
							for (var i of m.Parameters) {
								if (i in a0 || coding) {
									p[i] = a0[i];
								}
							}
						} else {
							[...arguments].filter(isValid).slice(0, m.Parameters.length).forEach((a, i) => p[m.Parameters[i]] = a);
						}
						for (var i in p) {
							u += `&${i}=${p[i]}`;
						}
					}
				}
				var cb = [...arguments, jo[m.Name], function () { }].filter(e=>typeof e === "function")[0];
				var r = new XMLHttpRequest();
				r.open(method, u);
				r.onload = e=> {
					Him.Done.call(this, r);
					this[ing] = false;
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
					if (typeof m["Return"] === "object" && typeof s === "string" && s) {
						[Him.Error, function () { }].filter(ee=>typeof ee === "function")[0].call(this, s);
					}
					cb.call(this, coding ? m["Return"] : s);
				};
				Him.Waiting.call(this, r);
				r.send(data);
				return r;
			};
		});
	}
	function findClass(co, jo) {
		for (var i in co) {
			var oi = co[i];
			var ji = i in jo ? jo[i] : {};
			if (oi instanceof Array) {
				makeClass(co, i, ji);
			} else {
				findClass(oi, ji);
			}
		}
	}
	for (var i in Him.CSharp) {
		if (i in Him.Javascript) {
			findClass(Him.CSharp[i], Him.Javascript[i]);
		}
	}
	Him.Ready = function (a) {
		if (coding) {
			cls.forEach(e=> {
				for (var i in e.prototype) {
					new e()[i]();
				}
			});
		}
		Him.Error = a && a.Error;
		Him.Waiting = a && a.Waiting;
		Him.Done = a && a.Done;
	};
	Him.Error = s=>console.log(s);
}
Him.CSharp = {};
Him.Javascript = {};
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