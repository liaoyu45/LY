﻿/// <reference path="../Gods/Him.js" />
/// <reference path="../Gods/CSharp/Me.js" />
/// <reference path="../Gods/Javascript/Me.js" />
/// <reference path="../Gods/His.js" />
/// <reference path="DailyViewModel.js" />
Him.Javascript.Me.I.FindMyself = function (e) {
	vm.I.FindMyself(!!e);
	vm.I.Name(e);
	if (e) {
		this.WakeUp();
	}
};
Him.Javascript.Me.I.Desire = function (e) {
	if (vm.DailyState.DesireCount() > 0) {
		vm.DailyState.DesireCount(vm.DailyState.DesireCount() - 1);
	}
	vm.PendingPlan.Required(vm.PendingPlan.Testing() ? e.Required : 0);
	(vm.PendingPlan.Testing() ? function () { } : this.ArrangePrepare)();
	this.ArrangePrepare
};
Him.Javascript.Me.I.Leave = () =>location.reload();
Him.Javascript.Me.I.WakeUp = function (e) {
	if (e) {
		for (var i in e) {
			if (i in vm.DailyState) {
				vm.DailyState[i](e[i]);
			}
		}
	}
	this.ArrangePrepare();
};
Him.Javascript.Me.I.ArrangeQuery = function (e) {
	e.forEach(ee=> {
		ee.Efforts = ko.observableArray(ee.Efforts);
		ee.Progress = ko.computed(() => ee.Done ? ee.DoneTime : ee.Percent.toFixed(2));
		ee.Percent = ko.observable(ee.Percent);
	});
	var id = (vm.CurrentPlan() || {}).Id;
	vm.Plans(e);
	vm.CurrentPlan(vm.Plans().filter(ee=>ee.Id === id)[0]);
};
Him.Javascript.Me.I.ArrangePrepare = function (e) {
	for (var i in e) {
		vm.PlansSetting[i](e[i]);
	}
	if (e.Total) {
		this.ArrangeQuery.last = () =>this.ArrangeQuery(e.Id, vm.PlansSetting.Start(), vm.PlansSetting.End());
		this.ArrangeQuery.last();
	}
};
Him.Javascript.Me.I.Pay = function (e) {
	this.ArrangeQuery.last();
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
(window.me = new Him.CSharp.Me.I()).FindMyself({ name: "", password: "" });
