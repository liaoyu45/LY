/// <reference path="../CSharp/Me.js" />
/// <reference path="../Him.js" />
/// <reference path="../../AllOfMe/Daily/ViewModel.js" />
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
		"Pay": function (e) {
			this.QueryEfforts(vm.CurrentPlan().Id, function () {
				vm.CurrentPlan().Abandoned(false);
				vm.CurrentPlan().Done(false);
			});
		},
		"Desire": function (e, r) {
			vm.PlansSkip(0);
			this.QueryPlans(function () {
				vm.CurrentPlan({ Content: r.thing.value, AppearTime: god.now, Id: e, Efforts: ko.observableArray(), Done: ko.observable(), Abandoned: ko.observable() });
			});
		},
		"GiveUp": function (e, r) {
			vm.Plans.remove(vm.CurrentPlan());
			vm.CurrentPlan(null);
		},
		"Finish": null,
		"Forget": function () {
			vm.CurrentPlan().Done(false);
			vm.CurrentPlan().Abandoned(true);
		},
		"QueryPlans": function (e) {
			if (!vm.PlansSkip()) {
				vm.Plans.removeAll();
			}
			e.forEach(ee=> {
				ee.Efforts = ko.observableArray(ee.Efforts);
				ee.Done = ko.observable(ee.Done);
				ee.Abandoned = ko.observable(ee.Abandoned);
			});
			vm.Plans(vm.Plans().concat(e));
		},
		"QueryEfforts": function (e, r) {
			vm.PendingEffort(null);
			vm.CurrentPlan(vm.Plans().filter(ee=>ee.Id === r.planId)[0]);
			if (!vm.EffortsSkip()) {
				vm.CurrentPlan().Efforts.removeAll();
			}
			vm.CurrentPlan().Efforts(vm.CurrentPlan().Efforts().concat(e));
		}
	}
});
