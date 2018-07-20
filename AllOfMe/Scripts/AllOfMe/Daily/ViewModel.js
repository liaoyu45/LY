/// <reference path="../../knockout-3.4.2.debug.js" />
window.vm = {
	Name: ko.observable(),
	Plans: ko.observableArray(),
	PlanStart: ko.observable("1990-03-13"),
	PlanEnd: ko.observable(god.now.split(' ')[0]),
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
	PlansTake: ko.observable(3),
	EffortsTake: ko.observable(3),
};
addEventListener("load", () => ko.applyBindings(vm));
