if (typeof AjaxRoute === "function") {//HashCode//GetHashCode
	AjaxRoute("/AjaxRoute", "AjaxKey", Value);
	delete AjaxRoute
	function FillFunc(from, to) {
		for (var i in from) {
			if (i in to) {
				var fi = from[i], ti = to[i];
				if (typeof fi === "object") {
					FillFunc(fi, ti.prototype || ti);
				} else {
					ti.AjaxKey = fi;
				}
			}
		}
	}
	if (window["Filename"]) {
		FillFunc(window["Filename"], window);
	}
	delete FillFunc
}