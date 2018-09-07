/// <reference path="http://localhost:53201/CSharp?Me" />
/// <reference path="/Script/Gods.Web.js" />
/// <reference path="ViewModel.js" />
god.Javascript.Me.I.Desire = function () {
	alert("done");
};
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
	var c = vm.Plans().filter(ee=>ee.Id === e.planId)[0].Content;
	return !confirm("永远放弃么？" + c);
};
god.Javascript.Me.I.Desire = e=> {
	return !confirm("想要？" + e.thing.value);
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