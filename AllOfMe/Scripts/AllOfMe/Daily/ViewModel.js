/// <reference path="../../knockout-3.4.2.debug.js" />
window.vm = {
	Name: ko.observable(),
	Plans: ko.observableArray(),
	PlanStart: ko.observable("1990-03-13"),
	PlanEnd: ko.observable(),
	EffortStart: ko.observable(),
	EffortEnd: ko.observable(),
	PlansSetting: {
		Id: ko.observable(),
		Total: ko.observable(0),
		ShowingDetail: ko.observable(true)
	},
	CurrentPlan: ko.observable(),
	PendingPlan: ko.observable(),
	PendingEffort: ko.observable(),
	PlansSkip: ko.observable(),
	EffortsSkip: ko.observable(),
	DoneState: ko.observable("all"),
	DoneStateText: ko.computed(function () {
		return "vm" in window && ({ "true": "已完成", "false": "未完成" })[vm.DoneState()] || "全部";
	})
};
addEventListener("load", () => ko.applyBindings(vm));
