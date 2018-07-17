/// <reference path="../../knockout-3.4.2.debug.js" />
window.vm = {
	Name: ko.observable(),
	Plans: ko.observableArray(),
	PlansSetting: {
		Id: ko.observable(),
		Total: ko.observable(0),
		ShowingDetail: ko.observable(true)
	},
	CurrentPlan: ko.observable(),
	PendingPlan: ko.observable(),
	PendingEffort: ko.observable(),
	PlansSkip: ko.computed(function () {
		return "vm" in window && vm.Plans().length;
	}),
	EffortsSkip: ko.computed(function () {
		"vm" in window && vm.CurrentPlan().Efforts().length
	}),
	PlansTake: ko.observable(8),
	EffortsTake: ko.observable(8),
	HaveMorePlans: ko.computed(() =>"vm" in window && vm.Plans().length % vm.PlansTake() === 0),
	HaveMoreEfforts: ko.computed(() =>"vm" in window && vm.CurrentPlan() && vm.CurrentPlan().Efforts().length % vm.EffortsTake() === 0)
};
addEventListener("load", () => ko.applyBindings(vm));
