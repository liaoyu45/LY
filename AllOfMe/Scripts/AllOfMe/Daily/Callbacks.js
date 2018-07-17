/// <reference path="../../Gods/CSharp/Me.js" />
/// <reference path="../../Gods/Him.js" />
/// <reference path="../../Gods/His.js" />
/// <reference path="ViewModel.js" />
/// <reference path="../../knockout-3.4.2.debug.js" />
god.Javascript.Me.I.WakeUp = function (e) {
	vm.Name(e);
	if (e) {
		this.QueryPlans();
	}
};
god.Javascript.Me.I.Desire = function (e) {
	this.QueryPlans();
};
god.Javascript.Me.I.Sleep = () =>location.reload();
god.Javascript.Me.I.QueryEfforts = function (e, r) {
	vm.CurrentPlan(vm.Plans().filter(ee=>ee.Id === r.planId)[0]);
	if (!vm.EffortsSkip()) {
		vm.CurrentPlan().Efforts.removeAll();
	}
	vm.CurrentPlan().Efforts(vm.CurrentPlan().Efforts().concat(e));
};
god.Javascript.Me.I.QueryPlans = function (e) {
	if (!vm.PlansSkip()) {
		vm.Plans.removeAll();
	}
	e.forEach(ee=>ee.Efforts = ko.observableArray(ee.Efforts));
	vm.Plans(vm.Plans().concat(e));
};
god.Javascript.Me.I.Pay = function (e) {
	vm.CurrentPlan().Efforts.push({ Content: vm.PendingEffort(), AppearTime: new Date() });
	vm.PendingEffort(null);
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
