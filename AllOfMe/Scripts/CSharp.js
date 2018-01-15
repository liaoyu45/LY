(function (thisArg, ajax, obj) {
	"user strict"
	var s = {};
	thisArg.split('.').forEach(i => {
		s = i in window ? window[i] : window[i] = {};
	});
	function makeClass(n, i) {
		var oi = n[i];
		var ps = (oi.Properties || []).map(e=> { return { Name: e.Name, Value: null }; });
		n[i] = function (data) {
			if (data) {
				for (var i of ps) {
					i.Value = data[i.Name] || null;
				}
			}
		};
		ps.forEach(p=>
			Object.defineProperty(n[i].prototype, p.Name, {
				get: () => p.Value,
				set: v=>p.Value = v,
				enumerable: true
			}));
		oi.Methods.forEach(m=> {
			var r;
			function then(m, u, a, t) {//method,url,data,onload
				r = r || new XMLHttpRequest();
				r.open(m, u);
				r.onload = t;
				r.send(a);
				return r;
			};
			function makeLater(t, later) {
				return function (e) {
					later.call(t, e, this);
					r = null;
				};
			};
			if (m.Queryable) {
				n[i].prototype[m.Name] = function () {
					var t = this,
						url = `?${ajax}=${m.Key}`;
					[...arguments].forEach((a, i) => url += `&${m.Parameters[i].Name}=${a}`);
					return {
						get: function (later) {
							return then("get", url, null, makeLater(t, later));
						}
					};
				}
			} else {
				n[i].prototype[m.Name] = function (form) {
					var t = this;
					form = new FormData(form);
					form.append(ajax, m.Key);
					return {
						post: function (later) {
							return then("post", "", form, makeLater(t, later));
						}
					};
				};
			}
			n[i].prototype[m.Name].prepare = function (request) {
				r = request();
				return n[i].prototype[m.Name];
			};
			if (location.href.length === 11) {
				n[i].prototype[m.Name]();
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
		s[i] = obj[i];
	}
})(
"window",
"AjaxKey811297106",
{
  "BLL": {
    "ILive": {
      "Properties": [
        {
          "Name": "Left",
          "Type": "System.Int32"
        }
      ],
      "Methods": [
        {
          "Name": "WakeUp",
          "Key": "19017142.-1089913253",
          "Parameters": [
            {
              "Name": "time",
              "Type": "System.Int32"
            }
          ]
        },
        {
          "Name": "Test",
          "Key": "19017142.816121540",
          "Queryable": true,
          "ReturnType": []
        }
      ]
    },
    "IWork": {
      "Methods": [
        {
          "Name": "GoWork",
          "Key": "16090703.566279348"
        }
      ]
    }
  }
}
);