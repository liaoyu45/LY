/// <reference path="../Gods/Him.js" />
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
	vm.PendingPlan.Required(e);
	if (!vm.PendingPlan.Testing()) {
		this.ArrangePrepare();
	}
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
	vm.Plans(e);
};
Him.Javascript.Me.I.ArrangePrepare = function (e) {
	for (var i in e) {
		vm.PlansSetting[i](e[i]);
	}
	if (e.Total) {
		this.ArrangeQuery(e.Id, vm.PlansSetting.Start(), vm.PlansSetting.End());
	}
};
Him.Javascript.Me.I.Pay = function (e) {
	if (e) {
		vm.CurrentPlan().Efforts.push({
			Content: vm.PendingEffort.Content(),
			Value: e
		});
		vm.PendingEffort.Content(null);
	}
};
onload = () => {
	Him.Ready();
	(window.me = new Him.CSharp.Me.I()).FindMyself({ name: "", password: ""});
};
