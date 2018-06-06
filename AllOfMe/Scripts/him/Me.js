(function (url, key, obj) {
	"user strict";
	var baseURL = "";
	function makeClass(n, i) {
		var oi = n[i];
		n[i] = function (data) {
			for (var i in data || {}) {
				this[i] = data[i];
			}
		};
		oi.Methods.forEach(m=> {
			var r;
			function then(m, u, a, t) {//method,url,data,onload
				r = r || new XMLHttpRequest();
				r.open(m, url + u);
				r.onload = t;
				r.send(a);
				return r;
			}
			if (typeof m["Return"] === "object") {
				/// <summary>get</summary>
				n[i].prototype[m.Name] = function () {
					var u = `?${key}=${m.Key}`;
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
					var cb = [...arguments].reverse()[0];
					cb = typeof cb === "function" ? cb : function () { };
					if (location.href.length === 11) {
						cb(m["Return"]);
					}
					return then("get", u, null, e=> {
						cb(JSON.parse(e.currentTarget.responseText));
					});
				};
			} else {
				/// <summary>post</summary>
				n[i].prototype[m.Name] = function (form) {
					form = new FormData(form);
					form.append(key, m.Key);
					var cb = typeof arguments[1] === "function" ? arguments[1] : function () { };
					if (location.href.length === 11) {
						cb(m["Return"]);
					}
					return then("post", "", form, e=>cb(e.currentTarget.responseText));
				};
			}
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
})(
"him",
"Him1344150689",
{
  "Soul": {
    "Methods": [
      {
        "Name": "Pay",
        "Key": "18475057.-1622356561",
        "Parameters": [
          "planId",
          "some"
        ],
        "Return": 0
      },
      {
        "Name": "Desire",
        "Key": "18475057.-314836140",
        "Parameters": [
          "thing"
        ],
        "Return": 0
      },
      {
        "Name": "Feel",
        "Key": "18475057.35221968",
        "Parameters": [
          "t"
        ],
        "Return": 0
      },
      {
        "Name": "GiveUp",
        "Key": "18475057.1297976969",
        "Parameters": [
          "planId"
        ],
        "Return": 0
      },
      {
        "Name": "Arrange",
        "Key": "18475057.959463723",
        "Parameters": [
          "start",
          "end",
          "min",
          "max",
          "pMin",
          "pMax",
          "done"
        ],
        "Return": [
          {
            "Required": 0,
            "Done": false,
            "Tag": null,
            "Efforts": [],
            "Id": 0,
            "AppearTime": "0001-01-01T00:00:00",
            "Content": null
          }
        ]
      }
    ]
  }
}
);
