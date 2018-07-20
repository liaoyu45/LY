﻿/// <reference path="../../Gods/CSharp/Me.js" />
/// <reference path="../../Gods/Him.js" />
/// <reference path="ViewModel.js" />
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
god.Javascript.Me.I.GiveUp = () => {
	return confirm("永远放弃么？");
};
addEventListener("keydown", e => {
	if (e.keyCode === 27) {
		vm.CurrentPlan(null);
	}
});
function toggleForms(parent, show) {
	if (parent.dataset.last === show.id) {
		parent.dataset.last = "";
		parent.style.display = "none";
	} else {
		parent.dataset.last = show.id;
		parent.style.display = "";
		[...show.parentElement.children].forEach(e=>e.style.display = "none");
		show.style.display = "";
	}
}
