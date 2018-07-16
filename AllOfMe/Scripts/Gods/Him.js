(function () {
	var coding = location.href.length === 11;
	var god = window.god || (window.god = {});
	"user strict";
	var ILikeCSharpSoI = "MakeJavasciptLookLikeCSharp";
	var settings = (function () {
		var r = { Key: "Him1344150689", Url: "/Gods" };
		var s = coding ? null : localStorage.getItem(ILikeCSharpSoI);
		try {
			return s ? JSON.parse(s) : r;
		} catch (e) {
		}
		localStorage.setItem(ILikeCSharpSoI, JSON.stringify(r));
		return r;
	})();
	var CSharp = god.CSharp || (god.CSharp = {});
	var Javascript = god.Javascript || (god.Javascript = {});
	var events = {};
	["waiting", "error", "done"].forEach(e=>events[e] = s=>console.log(s));
	function isValid(e) {
		return e instanceof String || !isNaN(e) || e instanceof Date || e instanceof Boolean;
	}
	var cls = [];
	function makeClass(obj, n, jo) {
		var oi = obj[n];
		Object.defineProperties(jo, (function () {
			var r = {};
			oi.map(m=>m.Name).forEach(i=> {
				r[i] = (function () {
					var arr = jo[i] instanceof Function ? [jo[i]] : [];
					return {
						get: () =>arr,
						set: v=>(v instanceof Function ? arr : []).push(v)
					};
				}());
			});
			return r;
		})());
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
				var ev = [...arguments].filter(e=>e instanceof Event)[0] || window.event;
				var a0 = arguments[0] || document.querySelector(`[data-him='${m.Name}']`);
				if (a0 instanceof HTMLElement) {//while coding, instanceof returns true
					while (!(a0 instanceof HTMLFormElement)) {
						a0 = a0.parentElement;
						if (!a0 || a0 === document.body) {
							console.log("Can not find an HTMLFormElement: " + m.Name);
							return;
						}
					}
				}
				var method = a0 instanceof HTMLFormElement,
					data = null,
					u = settings.Url,
					request;
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
					data = new FormData(request = a0);
					data.append(settings.Key, m.Key);
					for (var i in this) {
						if (i.constructor !== Symbol && isValid(this[i]) && !(i in a0)) {
							data.append(i, this[i]);
						}
					}
				} else {
					method = "get";
					u += `?${settings.Key}=${m.Key}`;
					request = {};
					if (m.Parameters) {
						for (let i in this) {
							if (m.Parameters.some(e=>e === i)) {
								request[i] = this[i];
							}
						}
						for (let i of m.Parameters) {
							if (a0 instanceof Object && i in a0 || coding) {
								request[i] = a0[i];
							}
						}
						[...arguments].filter(isValid).slice(0, m.Parameters.length).forEach((a, i) => request[m.Parameters[i]] = a);
						for (let i in request) {
							u += `&${i}=${request[i]}`;
						}
					}
				}
				var cb = [...arguments, ...jo[m.Name], function () { }].filter(e=>typeof e === "function");
				var r = new XMLHttpRequest();
				r.open(method, u);
				r.onabort = () =>this[ing] = false;
				r.onload = e=> {
					events.done.apply(this, [r, ev]);
					this[ing] = false;
					var s = r.responseText;
					if (s.toLowerCase() === "false") {
						s = false;
					} else if (s.toLowerCase() === "true") {
						s = true;
					} else if (s.length && !isNaN(s)) {
						s = (s.indexOf('.') > -1 ? parseFloat : parseInt)(s);
					} else if (new RegExp(/^[{\[].*[}\]]$/).test(s)) {
						try {
							s = JSON.parse(s);
						} catch (e) {
							console.log(e);
						}
					} else if (new RegExp(/^\d+-(1[012]|0?[1-9])-([12][0-9]|0?[1-9])$/).test(s)) {
						s = new Date(Date.parse(s));
					}
					if (typeof m["Return"] === "object" && typeof s === "string" && s) {
						[events.error, function () { }].filter(ee=>typeof ee === "function")[0].apply(this, [s, ev]);
						return;
					}
					cb.forEach(e=>e.apply(this, [coding ? m["Return"] : s, request, ev]));
				};
				if (this[obo]) {
					if (this[ing]) {
						return;
					}
					this[ing] = true;
				}
				events.waiting.apply(this, [r, ev]);
				r.send(data);
				return r;
			};
		});
	}
	function findClass(co, jo) {
		for (var i in co) {
			var oi = co[i];
			var ji = i in jo ? jo[i] : jo[i] = {};
			if (oi instanceof Array) {
				makeClass(co, i, ji);
			} else {
				findClass(oi, ji);
			}
		}
	}
	for (var i in CSharp) {
		if (!(i in Javascript)) {
			Javascript[i] = {};
		}
		findClass(CSharp[i], Javascript[i]);
	}
	god[ILikeCSharpSoI] = function (a) {
		if (coding) {
			cls.forEach(e=> {
				for (var i in e.prototype) {
					new e()[i]();
				}
			});
		}
		for (var i in events) {
			events[i] = a[i] || events[i];
		}
	};
})();