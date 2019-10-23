(function () {
	"user strict";
	function init() {
		var coding = location.href.length === 11;
		var ILikeCSharpSoI = "MakeJavasciptLookLikeCSharp";
		var CSharp = window.CSharp || (window.CSharp = {});
		var Javascript = window.Javascript || (window.Javascript = {});
		var events = {};
		var settings = { Key: "Him1344150689", Url: "/Gods" };
		["waiting", "error", "done"].forEach(e => events[e] = s => console.log(s));
		function isValid(e) {
			return ["number", "string", "boolean"].indexOf(typeof e) > -1 || [String, Number, Date, Boolean].some(a => e instanceof a);
		}
		function canApplyThis(f) {
			return typeof f === "function" && f.toString().split('{')[0].indexOf("=>") === -1;
		}
		function makeClass(n) {
			var oi = CSharp[n];
			var jo = {};
			Object.defineProperties(jo, (function () {
				var r = {};
				oi.map(m => m.Name).forEach(i => {
					var arr = jo[i] instanceof Function ? [jo[i]] : [];
					r[i] = {
						get: () => arr,
						set: v => {
							(v instanceof Function ? arr : []).push(v);
							if (coding) {
								new CSharp[n]()[i]();
							}
						},
						enumerable: true
					};
				});
				return r;
			})());
			Object.defineProperty(Javascript, n, {
				get: () => jo,
				set: v => {
					for (var i in v) {
						(i in jo ? jo : {})[i] = v[i];
					}
				},
				enumerable: true
			});
			var obo = Symbol();
			var progress = {};
			n = n[0] === "I" && n[1].toUpperCase() === n[1] ? n.substr(1) : n;
			CSharp[n] = function (data) {
				oi.forEach(e => progress[e.Name] = 0);
				Object.defineProperty(this, "data", { get: () => data });
			};
			oi.forEach(m => {
				var mn = m.Name,
					mp = m.Parameters,
					mk = m.Key;
				CSharp[n].prototype[mn] = function () {
					var ev = [...arguments].filter(e => e instanceof Event)[0] || window.event;
					var a0 = arguments.length ? [...arguments].filter(e => e instanceof HTMLElement)[0] : document.querySelector(`form[data-csharp='${mn}']`);
					if (a0 instanceof HTMLElement) {//while coding, "instanceof" returns true
						while (!(a0 instanceof HTMLFormElement)) {
							a0 = a0.parentElement;
							if (!a0 || a0 === document.body) {
								console.log("Can not find an HTMLFormElement: " + mn);
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
						data.append(settings.Key, mk);
					} else {
						method = "get";
						u += `?${settings.Key}=${mk}`;
						request = {};
						if (mp) {
							for (let i of mp) {
								if (a0 instanceof Object && i in a0 || coding) {
									request[i] = a0[i];
								}
							}
							[...arguments].filter(isValid).slice(0, mp.length).forEach((a, i) => request[mp[i]] = a);
							for (let i in request) {
								u += `&${i}=${request[i]}`;
							}
						}
					}
					var cb = jo[mn].filter(e => !canApplyThis(e));
					for (let i of cb) {
						try {
							if (i(request)) {
								return;
							}
						} catch (e) {
							return;
						}
					}
					cb = [...[...arguments].filter(e => typeof e === "function"), ...jo[mn].filter(canApplyThis)];
					var r = new XMLHttpRequest();
					r.withCredentials = true;
					r.open(method, u);
					r.onabort = () => progress[mn]++;
					r.onerror = r.onload = e => {
						events.done.apply(this, [r, ev, m]);
						progress[mn]++;
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
						if (progress[mn] % 2) {
							return;
						}
					}
					events.waiting.apply(this, [r, ev]);
					r.send(data);
					progress[mn]++;
					return r;
				};
			});
		}
		Object.getOwnPropertyNames(CSharp).forEach(makeClass);
		CSharp[ILikeCSharpSoI] = function (globleCallbacks) {
			for (var i in events) {
				events[i] = [globleCallbacks[i], events[i]].filter(e => e instanceof Function)[0];
			}
		};
	}
	var src = [...document.scripts].filter(e => e.src.toLowerCase().split('?')[0].endsWith("gods.web.js"))[0].src.split('?')[1];
	if (src) {
		var s = document.createElement("script");
		s.src = "CSharp?" + src;
		s.onload = init;
		document.head.appendChild(s);
	} else {
		init();
	}
})();