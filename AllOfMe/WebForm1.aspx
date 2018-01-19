<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Me" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script src="Him.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div>
			<input type="text" name="Left" value="989248" />
			<input type="text" name="time" value="123123" />
		</div>
		<input type="button" name="name" value="query" onclick="qqqqqq()" />
		<input type="button" name="name" value="post" onclick="pppp();" />
	</form>
	<script type="text/javascript">
		function pppp() {
			new him.BLL.IWork().GoWork(form1).post(function (a, aa) {
				alert(aa.responseText);
			});
		}
		function qqqqqq() {
			new him.BLL.ILive({ Left: 3 }).Test().get(function (a, aa) {
				alert(aa.responseText);
			});
		}
		var e = {};
		e.hasOwnProperty("")
	</script>
	w289034234
</body>
</html>
