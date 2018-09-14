/// <reference path="http://localhost:53201/CSharp?Me.js" />
/// <reference path="/Scripts/Gods.Web.js" />
/// <reference path="/Scripts/knockout-3.4.2.debug.js" />
window.vm = {
	Name: ko.observable(),
	Plans: ko.observableArray(),
	PlanStart: ko.observable(),
	CurrentPlan: ko.observable(),
	PendingPlan: ko.observable(),
	PendingEffort: ko.observable(),
	DoneState: ko.observable("")
};
addEventListener("load", () => ko.applyBindings(vm));
god.Javascript.Me.Inside.Soul = {
	WakeUp: function (e) {
		vm.Name(e);
		if (e) {
			this.QueryPlans();
		}
	},
	"Sleep": function () {
		location.reload();
	},
	"Pay": function (e, r) {
		vm.CurrentPlan().Efforts.push({
			AppearTime: god.now,
			Content: vm.PendingEffort(),
			Id: e
		});
		vm.PendingEffort(null);
		if (r.done.checked) {
			vm.CurrentPlan().DoneTime(new Date());
		}
		r.content.focus();
	},
	"Desire": function (e, r) {
		var v = r.thing.value;
		vm.PendingPlan(null);
		this.QueryPlans();
	},
	"GiveUp": function (e, r) {
		vm.Plans.remove(vm.Plans().filter(a=>a.Id === r.planId)[0]);
		vm.CurrentPlan(null);
	},
	"QueryPlans": function (e) {
		e.forEach(ee=> {
			ee.Efforts = ko.observableArray(ee.Efforts);
			ee.DoneTime = ko.observable(ee.DoneTime);
		});
		vm.Plans(e);
	},
	"QueryEfforts": function (e, r) {
		vm.CurrentPlan(vm.Plans().filter(ee=>ee.Id === r.planId)[0]);
		vm.CurrentPlan().Efforts(e);
	}
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
(window.me = new god.CSharp.Me.Inside.Soul()).WakeUp();
god.Javascript.Me.Inside.Soul.GiveUp = (e) => {
	var c = vm.Plans().filter(ee=>ee.Id === e.planId)[0].Content;
	return !confirm("永远放弃么？" + c);
};
god.Javascript.Me.Inside.Soul.Desire = e=> {
	return !confirm("想要？" + e.thing.value);
};
