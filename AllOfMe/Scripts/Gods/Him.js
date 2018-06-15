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
				var cb = [[...arguments].reverse()[0], (function (s) {
					var j = Him.Javascript;
					path.split(',').forEach(e=>j = j[e]);
					return j[m.Name];
				})(this)].filter(e=>typeof e === "function")[0] || function () { };
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

//Him.CSharp = {
//	"I": {
//		"Methods": [
//		  {
//		  	"Name": "MyAverageFeeling",
//		  	"Key": "54848996.1703672582",
//		  	"Return": 0
//		  },
//		  {
//		  	"Name": "Pay",
//		  	"Key": "54848996.1622356561",
//		  	"Parameters": [
//			  "planId",
//			  "some"
//		  	],
//		  	"Return": 0
//		  },
//		  {
//		  	"Name": "Desire",
//		  	"Key": "54848996.314836140",
//		  	"Parameters": [
//			  "thing"
//		  	],
//		  	"Return": 0
//		  },
//		  {
//		  	"Name": "Feel",
//		  	"Key": "54848996.150982569",
//		  	"Parameters": [
//			  "content",
//			  "tag",
//			  "value",
//			  "appearTime",
//			  "planId"
//		  	],
//		  	"Return": 0
//		  },
//		  {
//		  	"Name": "GiveUp",
//		  	"Key": "54848996.1297976969",
//		  	"Parameters": [
//			  "planId"
//		  	],
//		  	"Return": 0
//		  },
//		  {
//		  	"Name": "Awake",
//		  	"Key": "54848996.1123722116",
//		  	"Parameters": [
//			  "name"
//		  	],
//		  	"Return": 0
//		  },
//		  {
//		  	"Name": "Arrange",
//		  	"Key": "54848996.807085193",
//		  	"Parameters": [
//			  "start",
//			  "end",
//			  "min",
//			  "max",
//			  "pMin",
//			  "pMax",
//			  "done"
//		  	],
//		  	"Return": [
//			  {
//			  	"Required": 0,
//			  	"Done": false,
//			  	"Tag": null,
//			  	"Efforts": [],
//			  	"Content": null,
//			  	"Id": 0,
//			  	"AppearTime": "0001-01-01T00:00:00"
//			  }
//		  	]
//		  }
//		]
//	}
//}
//Him.Javascript = {
//	"I": {
//		"MyAverageFeeling": null,
//		"Pay": null,
//		"Desire": null,
//		"Feel": null,
//		"GiveUp": null,
//		"Awake": null,
//		"Arrange": null
//	}
//}

//addEventListener("load", () => {
//	Him.Javascript.I.Arrange = e=> { };
//	Him('Gods', 'Him1344150689');
//});