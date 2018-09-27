(function () {
	"user strict";
	var coding = location.href.length === 11;
	var god = window.god || (window.god = {});
	var ILikeCSharpSoI = "MakeJavasciptLookLikeCSharp";
	var CSharp = god.CSharp || (god.CSharp = {});
	var Javascript = god.Javascript || (god.Javascript = {});
	var events = {};
	var settings = (function () {
		var r = { Key: "Him1344150689", Url: "/Gods" };
		var s = coding ? null : localStorage.getItem(ILikeCSharpSoI);
		try {
			return s ? JSON.parse(s) : r;
		} catch (e) {
			console.log("can not be");
		}
		localStorage.setItem(ILikeCSharpSoI, JSON.stringify(r));
		return r;
	})();
	var cls = [];
	["waiting", "error", "done"].forEach(e=>events[e] = s=>console.log(s));
	function isValid(e) {
		return ["number", "string", "boolean"].indexOf(typeof e) > -1 || [String, Number, Date, Boolean].some(a=>e instanceof a);
	}
	function canApplyThis(f) {
		return typeof f === "function" && f.toString().split('{')[0].indexOf("=>") === -1;
	}
	function makeClass(obj, n, joi) {
		var oi = obj[n];
		var jo = {};
		Object.defineProperties(jo, (function () {
			var r = {};
			oi.map(m=>m.Name).forEach(i=> {
				var arr = jo[i] instanceof Function ? [jo[i]] : [];
				r[i] = {
					get: () =>arr,
					set: v=> {
						(v instanceof Function ? arr : []).push(v);
						if (coding) {
							new obj[n]()[i]();
						}
					},
					enumerable: true
				};
			});
			return r;
		})());
		Object.defineProperty(joi, n, {
			get: () =>jo,
			set: v=> {
				for (var i in v) {
					(i in jo ? jo : {})[i] = v[i];
				}
			},
			enumerable: true
		});
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
				var a0 = arguments.length ? [...arguments].filter(e=>e instanceof HTMLElement)[0] : document.querySelector(`form[data-csharp='${m.Name}']`);
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
					request = data = new FormData(a0);
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
				var cb = jo[m.Name].filter(e=>!canApplyThis(e));
				for (let i of cb) {
					try {
						if (i(request)) {
							return;
						}
					} catch (e) {
						return;
					}
				}
				cb = [...[...arguments].filter(e=>typeof e ==="function"), ...jo[m.Name].filter(canApplyThis)];
				var r = new XMLHttpRequest();
				r.withCredentials = true;
				r.open(method, u);
				r.onabort = () =>this[ing] = false;
				r.onerror = r.onload = e=> {
					events.done.apply(this, [r, ev, m]);
					this[ing] = false;
					var s = r.responseText.trim();
					if (s.toLowerCase() === "false") {
						s = false;
					} else if (s.toLowerCase() === "true") {
						s = true;
					} else if (s.length && !isNaN(s)) {
						s = (s.indexOf('.') > -1 ? parseFloat : parseInt)(s);
					} else if (new RegExp(/^\d+-(1[012]|0?[1-9])-([12][0-9]|0?[1-9])$/).test(s)) {
						s = new Date(Date.parse(s));
					} else if (s && typeof m["Return"] === "object") {
						try {
							s = JSON.parse(s);
						} catch (e) {
							console.log(e);
							console.log(s);
						}
					}
					var badArgs = [s, ev, m];
					if ("Return" in m) {
						if (typeof m["Return"] !== "string" && typeof s === "string" && s) {
							events.error.apply(this, badArgs);
							return;
						}
					} else {
						if (s) {
							events.error.apply(this, badArgs);
							return;
						}
					}
					for (var i of cb) {
						try {
							if (i.apply(this, [s || m["Return"], request, ev])) {
								break;
							}
						} catch (e) {
							console.log(e);
						}
					}
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
				makeClass(co, i, jo);
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
	god[ILikeCSharpSoI] = function (globleCallbacks) {
		for (var i in events) {
			events[i] = [globleCallbacks[i], events[i]].filter(e=>e instanceof Function)[0];
		}
	};
	if (!("exists" in god)) {
		for (var i in god) {
			window[i] = god[i];
		}
	}
})();