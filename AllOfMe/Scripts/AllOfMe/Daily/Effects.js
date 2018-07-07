/// <reference path="ViewModel.js" />
addEventListener("keydown", e => {
	if (e.keyCode === 27) {
		vm.CurrentPlan(null);
		vm.CurrentEffort(null);
	}
});
vm.SetCurrentPlan = function (i) {
	vm.CurrentPlan(i);

};
