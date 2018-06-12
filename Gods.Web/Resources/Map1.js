"use strict";
if (typeof AjaxRoute === "function") {//HashCode//GetHashCode
	AjaxRoute("/AjaxRoute", "AjaxKey", Value);
}
function AjaxKey(from, to) {
	for (var i in from) {
		if (i in to) {
			var fi = from[i], ti = to[i];
			if (typeof fi === "object") {
				AjaxKey(fi, ti.prototype || ti);
			} else {
				ti.AjaxKey = fi;
				if (location.href.length === 11) {
					fi(to["AjaxKey"].Methods.filter(e=>e.Name === i)[0]["Return"]);
				}
			}
		}
	}
}
addEventListener("load", function () {
	if (window["Filename"]) {
		AjaxKey(window["Filename"], window);
	}
});