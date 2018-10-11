/// <reference path="http://localhost:53201/CSharp?Me.js" />
/// <reference path="/Scripts/Gods.Web.js" />
/// <reference path="/Scripts/knockout-3.4.2.debug.js" />

/* 
本脚本库的主要目标：
像写C#一样写js，同步不同语言的编写人员编程风格。
减少前端开发人员对服务器功能开发进度的等待时间。
仅使用回调函数即可完成一个html页面所需的全部数据交互；
更多的js提示信息；
前端的功能注入；
阻止用户的重复网络请求操作；
使用C#上函数注释来省去接口文档的编写时间；
*/

/*
构造函数可使用2个参数。
如果其中第一个参数的类型为boolean，那么这个参数决定了此实例发起的所有请求是否为锁定请求，默认为锁定请求。
第二个（或第一个）参数类型应为object，其属性将复制到实例的对象，并在此实例发起的所有请求中使用这此属性。
*/
(window.me = new god.CSharp.Me.Inside.Soul()).WakeUp();

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
god.Javascript.Me.Inside.Soul.GiveUp = (e) => {
	var c = vm.Plans().filter(ee=>ee.Id === e.planId)[0].Content;
	return !confirm("永远放弃么？" + c);
};
god.Javascript.Me.Inside.Soul.Desire = e=> {
	return !confirm("想要？" + e.thing.value);
};
