<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Me" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script src="/him?Me=recreate"></script>
	<script src="/Scripts/him/Me.js"></script>
</head>
<body>
	<script src="god.js"></script>
	<form action="/" method="post" id="fff">
		<label>
			I want:
			<input type="text" name="thing" value="12839" /></label><input type="button" data-god="form: Soul.Desire" value="confirm" data-bind="click: Desire" />
		<fieldset>
			<legend></legend>
			<label>I need to pay: <span></span>(already/total)</label>
			<label>
				This time I will pay:
			<input type="range" min="1" max="111" name="effort" /></label><input type="button" value="confirm" />
		</fieldset>
		<label>
			Then I got:<span></span>
		</label>
	</form>
	<dl>
		<dt>My plans:</dt>
		<!--ko foreach:plans-->
		<dd>
			<dl>
				<dt data-bind="text: Content"></dt>
				<!--ko foreach:Efforts-->
				<dd data-bind="text: Content"></dd>
				<!--/ko-->
			</dl>
		</dd>
		<!--/ko-->
	</dl>
	<dl>
		<dt>I have payed:</dt>
		<dd></dd>
	</dl>
	<script type="text/javascript">
		//var obj = {
		//	thing: ko.observable(),
		//	Desire: function () {
		//		new Me.Soul().Desire(document.forms[0], function () {
		//			console.log(arguments[0]);
		//		});
		//	},
		//	plans: ko.observableArray()
		//};
		//ko.applyBindings(obj);
		[...document.querySelectorAll("input[type=button]")].filter(e=>e.dataset.god).forEach(e=> {
			var god = {};
			e.dataset.god.split(/:|,/).forEach(e=>{
				if (god["last"]) {
					god[god["last"]] = e.trim();
					god["last"] = "";
				} else {
					god["last"] = e;
				}
			});
			if (!god.form) {
				return;
			}
			var later = window;
			(god.later || "").split('.').forEach(e=> {
				later = later[e];
			});
			if (later === window) {
				later = function () { };
			}
			var form = e.parentElement;
			while (!(form instanceof HTMLFormElement)) {
				form = form.parentElement;
			}
			e.onclick = () => {
				var t = window,
					ev = false;
				[...god.form.split('.')].forEach(e=> {
					if (typeof t[e] === "function") {
						if (ev) {
							t[e](form, later);
						} else {
							ev = t = new t[e]();
						}
					} else {
						t = t[e];
					}
				});
			};
		});
	</script>
</body>
</html>
