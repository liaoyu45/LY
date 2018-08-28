/// <reference path="/Scripts/CSharp/Me.js" />
/// <reference path="/Scripts/god.web.js" />
/// <reference path="/Scripts/AllOfMe/Daily/ViewModel.js" />
god.MakeJavasciptLookLikeCSharp("Me", {
	"I": {
		"WakeUp": function (e) {
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
			vm.CurrentPlan().Done(false);
		},
		"Desire": function (e, r) {
			vm.PlansSkip(0);
			var v = r.thing.value;
			vm.PendingPlan(null);
			this.QueryPlans(function () {
				vm.CurrentPlan({ Content: v, AppearTime: god.now, Id: e, Efforts: ko.observableArray(), Done: ko.observable() });
				vm.EffortStart(god.now);
			});
		},
		"GiveUp": function (e, r) {
			vm.Plans.remove(vm.CurrentPlan());
			vm.CurrentPlan(null);
		},
		"Finish": function () {
			vm.CurrentPlan().Done(true);
		},
		"QueryPlans": function (e) {
			if (!vm.PlansSkip()) {
				vm.Plans.removeAll();
			}
			e.forEach(ee=> {
				ee.Efforts = ko.observableArray(ee.Efforts);
				ee.Done = ko.observable(ee.Done);
			});
			vm.Plans(vm.Plans().concat(e));
		},
		"QueryEfforts": function (e, r) {
			vm.CurrentPlan(vm.Plans().filter(ee=>ee.Id === r.planId)[0]);
			vm.EffortStart(vm.CurrentPlan().AppearTime);
			if (!vm.EffortsSkip()) {
				vm.CurrentPlan().Efforts.removeAll();
			}
			vm.CurrentPlan().Efforts(vm.CurrentPlan().Efforts().concat(e));
		}
	}
});
