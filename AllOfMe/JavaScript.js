
var e = {};
e.qwer = null;
function myfunction() {
	e.qwer(1);
	for (var i in e.qwer) {
		var aaa = i !== "ee" ? "" : 1;
	}
}
e.qwer = function (aa) {
	console.log(123);
}
e.qwer.ee = 1;
myfunction();