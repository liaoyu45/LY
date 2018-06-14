/// <reference path="Map.js" />
"use strict";
var ak = "";
if ("CSharpSettings" in CSharp) {
	var c = CSharp["CSharpSettings"];
	for (var obj of window) {
		if (obj !== window && "CSharpSettings" in obj) {
			CSharp(c.AjaxRoute, c.AjaxKey, obj);
		}
	}
}
addEventListener("load", function () {
	function load(from, to) {
		for (var i in from) {
			if (i in to) {
				var fi = from[i], ti = to[i];
				if (typeof fi === "object") {
					load(fi, ti.prototype || ti);
				} else {
					ti[ak] = fi;
					if (location.href.length === 11) {
						fi(to[ak].Methods.filter(e=>e.Name === i)[0]["Return"]);
					}
				}
			}
		}
	}
	for (var i of window) {
		if (i !== window && ak in i) {
			load(i, window);
		}
	}
});