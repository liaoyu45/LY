<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Me" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script type="text/javascript">
		var Me = {
			Soul: {
				Desire: function (e) {
					step0.style.display = "none";
					step1.style.display = "block";

					step1.need.max = e;
				},
				Arrange: function (ee) {
					ee[0].Content
				},
				Suicide: function () {
					location.reload();
				}
			},
			TestNamespace: {
				TestI: {
					GGGGG: function (i) {
						console.log(i + "wlkerjwlekjrwklrjrjr");
					}
				}
			}
		};
	</script>
	<script src="/Scripts/Gods/Gods.js"></script>
	<script src="/Gods?Me"></script>
	<script src="/Scripts/Gods/Me.js"></script>
	<style type="text/css">
		form + form {
			display: none;
		}
	</style>
</head>
<body>
	<script src="god.js"></script>
	<form id="step0">
		<label>
			I want:
			<input type="text" name="thing" value="12839" /></label>
		<input type="button" data-god="Soul.Desire" value="confirm" />
	</form>
	<form id="step1">
		<fieldset id="eee">
			<legend id="thing"></legend>
			<label>I need to pay at least: 
				<input type="range" name="need" value="0" max="" disabled />
			</label>
			<br />
			<label>
				This time I will pay:
			<input type="range" min="1" max="111" name="effort" /></label>
			<input type="button" value="confirm" data-god="form: Soul.Pay" />
			<input type="button" value="give up" data-god="form: Soul.GiveUp" />
		</fieldset>
	</form>
	<form>
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
		<dt onclick="myfunction();">I have payed:</dt>
		<dd></dd>
	</dl>
	<script type="text/javascript">
		function myfunction() {
			new TestNamespace.TestI().GGGGG(1);
		}
	</script>
</body>
</html>
i