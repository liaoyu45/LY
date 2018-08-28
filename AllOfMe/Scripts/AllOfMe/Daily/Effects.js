/// <reference path="../../CSharp/Me.js" />
/// <reference path="../../god.web.js" />
god.MakeJavasciptLookLikeCSharp({
	error: function (s) {
		alert(s);
	},
	waiting: function (r) {
		document.title = "wait please";
	},
	done: function (r) {
		document.title = "DONE";
	}
});
(window.me = new god.CSharp.Me.I()).WakeUp();
god.Javascript.Me.I.GiveUp = (e) => {
	return confirm("永远放弃么？");
};
addEventListener("keydown", e => {
	if (e.keyCode === 27) {
		vm.CurrentPlan(null);
	}
});
function toggleForms(show) {
	var parent = show.parentElement;
	parent[parent.firstElementChild === show ? "appendChild" : "insertBefore"](show, parent.firstElementChild);
}