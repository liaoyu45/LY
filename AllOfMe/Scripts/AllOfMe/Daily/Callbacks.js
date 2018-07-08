/// <reference path="../../Gods/Him.js" />
/// <reference path="../../Gods/CSharp/Me.js" />
/// <reference path="../../Gods/His.js" />
/// <reference path="ViewModel.js" />
/// <reference path="../../knockout-3.4.2.debug.js" />
Him.Javascript.Me.I.FindMyself = function (e) {
	vm.I.FindMyself(!!e);
	vm.I.Name(e);
	if (e) {
		this.WakeUp(() =>this.QueryPlans());
	}
};
Him.Javascript.Me.I.Desire = function (e) {
	this.QueryPlans();
};
Him.Javascript.Me.I.Leave = () =>location.reload();
Him.Javascript.Me.I.WakeUp = function (e) {
	vm.DailyState(e);
};
Him.Javascript.Me.I.QueryEfforts = function (e, r) {
	vm.CurrentPlan(vm.Plans().filter(ee=>ee.Id == r.planId)[0]);
	vm.CurrentPlan().Efforts(e);
};
Him.Javascript.Me.I.QueryPlans = function (e) {
	var id = (vm.CurrentPlan() || {}).Id;
	vm.Plans(e);
	vm.Plans().forEach(e=>e.Efforts = ko.observableArray(e.Efforts));
};
Him.Javascript.Me.I.Pay = function (e) {
	vm.CurrentPlan().Efforts.push({ Content: vm.PendingEffort(), AppearTime: new Date() });
	vm.PendingEffort(null);
};
Him.SetEvents({
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
(window.me = new Him.CSharp.Me.I()).FindMyself();
