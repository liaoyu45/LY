/// <reference path="../knockout-3.4.2.js" />
var vm = {
	I: {
		FindMyself: ko.observable(false),
		Name: ko.observable()
	},
	DailyState: {
		Energy: ko.observable(0),
		Content: ko.observable()
	},
	Plans: ko.observableArray(),
	PlansSetting: {
		Start: ko.observable(0),
		End: ko.observable(10),
		Size: ko.computed(function () { return vm && vm.PlansSetting.End() - vm.PlansSetting.Start(); }),
		Id: ko.observable(),
		Total: ko.observable(0)
	},
	CurrentPlan: ko.observable(),
	PendingPlan: {
		Content: ko.observable(),
		Required: ko.observable(),
		Testing: ko.observable(true)
	},
	PendingEffort: {
		Content: ko.observable(),
		Value: ko.observable()
	},
	QueryingPlans: ko.observable()
};
addEventListener("load", function () {
	ko.applyBindings(vm);
});
