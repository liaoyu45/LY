/// <reference path="../Gods/Him.js" />
/// <reference path="../Gods/CSharp/Me.js" />
/// <reference path="../Gods/Javascript/Me.js" />
/// <reference path="../Gods/His.js" />
/// <reference path="DailyViewModel.js" />
onload = () => {
	new I().FindMyself();
};
Me.I.Arrange = e=> {
	alert(e[0].AppearTime);
};
Me.I.FindMyself = e=> {
	vm.I.FindMyself(!!e);
	vm.I.Name(e);
	if (e) {
		new I().Awake();
	}
};
Me.I.Leave = () => {
	vm.I.Name("");
	vm.I.FindMyself(false);
};
Me.I.Awake = e=> {
	if (e) {
		for (var i in e) {
			if (i in vm.DailyState) {
				vm.DailyState[i](e[i]);
			}
		}
	}
};

