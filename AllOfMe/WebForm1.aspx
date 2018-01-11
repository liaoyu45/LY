<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Me" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script src="Him.js"></script>
</head>
<body>
	<script type="text/javascript">
		new BLL.IWork().GoWork().post(function (a, aa) {
			alert(aa.responseText);
		});
		//new BLL.ILive().Test().get(function (a, aa) {
		//	alert(aa.responseText);
		//});
	</script>
	w289034234
    <form id="form1" runat="server">
		<div>
		</div>
	</form>
</body>
</html>
