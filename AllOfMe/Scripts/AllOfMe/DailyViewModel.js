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
	CurrentPlan: ko.observable(),
	CurrentPlanContent: ko.observable()
};
addEventListener("load", function () {
	ko.applyBindings(vm);
});
