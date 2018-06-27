/// <reference path="../Gods/Him.js" />
/// <reference path="../Gods/CSharp/Me.js" />
/// <reference path="../Gods/Javascript/Me.js" />
/// <reference path="../Gods/His.js" />
/// <reference path="DailyViewModel.js" />
onload = () => {
	Me.I.FindMyself = function (e) {
		vm.I.FindMyself(!!e);
		if (e) {
			this.WakeUp();
			this.ArrangePrepare();
		}
	};
	new I().FindMyself();
	Me.I.Desire = function (e) {
		for (var i in e) {
			vm.PendingPlan[i](e[i]);
		}
		if (!vm.PendingPlan.Testing()) {
			this.ArrangePrepare();
		}
	};
	Me.I.Leave = () =>location.reload();
	Me.I.WakeUp = function (e) {
		if (e) {
			for (var i in e) {
				if (i in vm.DailyState) {
					vm.DailyState[i](e[i]);
				}
			}
		}
	};
	Me.I.ArrangeQuery = function (e) {
		vm.Plans(e);
	};
	Me.I.ArrangePrepare = function (e) {
		for (var i in e) {
			vm.PlansSetting[i](e[i]);
		}
		if (e.Total) {
			this.ArrangeQuery(e.Id, vm.PlansSetting.Start(), vm.PlansSetting.End());
		}
	};
};
