"use strict";
if (typeof Gods === "function") {//HashCode//38294922
	Gods("/Gods", "Him1344150689", {
  "Soul": {
    "Methods": [
      {
        "Name": "MyAverageFeeling",
        "Key": "32057793.1703672582",
        "Return": 0
      },
      {
        "Name": "Pay",
        "Key": "32057793.1622356561",
        "Parameters": [
          "planId",
          "some"
        ],
        "Return": 0
      },
      {
        "Name": "Desire",
        "Key": "32057793.314836140",
        "Parameters": [
          "thing"
        ],
        "Return": 0
      },
      {
        "Name": "Feel",
        "Key": "32057793.189759481",
        "Parameters": [
          "content",
          "tag",
          "value",
          "last",
          "planId"
        ],
        "Return": 0
      },
      {
        "Name": "GiveUp",
        "Key": "32057793.1297976969",
        "Parameters": [
          "planId"
        ],
        "Return": 0
      },
      {
        "Name": "Arrange",
        "Key": "32057793.868162613",
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
            "Content": null,
            "Id": 0,
            "AppearTime": "0001-01-01T00:00:00"
          }
        ]
      }
    ]
  },
  "TestNamespace": {
    "TestI": {
      "Methods": [
        {
          "Name": "GGGGG",
          "Key": "46067993.495303042",
          "Parameters": [
            "i"
          ],
          "Return": 0
        }
      ]
    }
  }
});
}
function Him1344150689(from, to) {
	for (var i in from) {
		if (i in to) {
			var fi = from[i], ti = to[i];
			if (typeof fi === "object") {
				Him1344150689(fi, ti.prototype || ti);
			} else {
				ti.Him1344150689 = fi;
				if (location.href.length === 11) {
					fi(to["Him1344150689"].Methods.filter(e=>e.Name === i)[0]["Return"]);
				}
			}
		}
	}
}
addEventListener("load", function () {
	if (window["Me"]) {
		Him1344150689(window["Me"], window);
	}
});