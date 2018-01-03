(function (thisArg, ajax, obj) {
	var s = {};
	thisArg.split('.').forEach(i => {
		s = i in window ? window[i] : window[i] = {};
	});
	function makeClass(n, i) {
		var oi = n[i];
		var ps = (oi.Properties || []).map(e=> { return { Name: e.Name, Value: null }; });
		n[i] = function (data) {
			for (var i of ps) {
				i.Value = data[i.Name] || null;
			}
		};
		ps.forEach(p=>
			Object.defineProperty(n[i].prototype, p.Name, {
				get: () => p.Value,
				set: v=>p.Value = v,
				enumerable: true
			}));
		oi.Methods.forEach(m=> {
			var ps = m.Parameters;
			n[i].prototype[m.Name] = function (args) {
				return function (later) {
					var url = "";
					var md;
					if (args instanceof HTMLFormElement) {
						md = "post";
						args = new FormData(args);
					} else {
						url += `?${ajax}=${m.Key}`;
						[...arguments].forEach((a, i) => url += `&${ps[i].Name}=${a}`);
						args = null;
						md = "get";
					}
					var r = new XMLHttpRequest();
					r.open(md, url);
					r.onload = () => {

					};
					r.send(args);
				};
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
		s[i] = obj[i];
	}
})(
"window",
"54d6287ce44341d291492c06be9e9090",
{
  "BLL": {
    "V0": {
      "Live": {
        "Methods": [
          {
            "Name": "WakeUp",
            "Key": 39070558
          }
        ]
      },
      "Work": {
        "Properties": [
          {
            "Name": "Folder",
            "Type": "System.String"
          }
        ],
        "Methods": [
          {
            "Name": "GoWork",
            "Key": 10598606
          }
        ]
      }
    }
  }
}
);
