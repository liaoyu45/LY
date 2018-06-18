/// <reference path="../Gods/Him.js" />
/// <reference path="../Gods/CSharp/Me.js" />
/// <reference path="../Gods/Javascript/Me.js" />
/// <reference path="../Gods/His.js" />
Me.I.Arrange = e=> {
	alert(e[0].AppearTime);
};
Me.I.FindMyself = e=> {
	if (!e) {
		return alert("另有人使用了这个名字");
	}
};
Him.Javascript.I.Awake = e=> {
	alert(e);
};
