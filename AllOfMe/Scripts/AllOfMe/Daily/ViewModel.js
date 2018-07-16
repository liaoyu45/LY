/// <reference path="../../knockout-3.4.2.debug.js" />
var vm = {
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
	QueryingPlans: ko.observable()
};
addEventListener("load", () =>ko.applyBindings(vm));
