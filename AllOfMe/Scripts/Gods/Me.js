if (typeof Gods === "function") {//HashCode//17848954
	Gods("/Gods", "Him1344150689", {
  "Soul": {
    "Methods": [
      {
        "Name": "Pay",
        "Key": "18475057.1622356561",
        "Parameters": [
          "planId",
          "some"
        ],
        "Return": 0
      },
      {
        "Name": "Desire",
        "Key": "18475057.314836140",
        "Parameters": [
          "thing"
        ],
        "Return": 0
      },
      {
        "Name": "Feel",
        "Key": "18475057.200913946",
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
        "Key": "18475057.1297976969",
        "Parameters": [
          "planId"
        ],
        "Return": 0
      },
      {
        "Name": "Arrange",
        "Key": "18475057.1004182937",
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
});
}
(function () {
	function FillFunc(from, to) {
		for (var i in from) {
			if (i in to) {
				var fi = from[i], ti = to[i];
				if (typeof fi === "object") {
					FillFunc(fi, ti.prototype || ti);
				} else {
					ti.Him1344150689 = fi;
				}
			}
		}
	}
	FillFunc(window[[...document.scripts].map(e=>e.src).filter(e=>e.indexOf(location.host + "/Gods?") > -1)[0].split("?")[1]], window);
})();