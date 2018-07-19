/// <reference path="C:\Users\zy\Source\Repos\LY\AllOfMe\god.js" />
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
god.Javascript.Me.I.Sleep = function () {
	location.reload();
};
god.Javascript.Me.I.Desire = function (e, r) {
	vm.PlansSkip(0);
	this.QueryPlans(function () {
		vm.CurrentPlan({ Content: r.thing.value, AppearTime: god.now, Id: e, Efforts: ko.observableArray() });
	});
};
god.Javascript.Me.I.QueryEfforts = function (e, r) {
	vm.PendingEffort(null);
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
	e.forEach(ee=> {
		ee.Efforts = ko.observableArray(ee.Efforts);
		ee.Done = ko.observable(ee.Done);
	});
	vm.Plans(vm.Plans().concat(e));
};
god.Javascript.Me.I.Pay = function (e) {
	this.QueryEfforts(vm.CurrentPlan().Id);
};
god.Javascript.Me.I.Abandon = () => {
	return confirm("永远放弃这个计划，不再想起么？");
};
god.Javascript.Me.I.Abandon = function () {

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
