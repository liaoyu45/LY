<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Me" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<%--<script src="Scripts/linq.js"></script>--%>
	<%--<script src="Him.js"></script>--%>
	<style type="text/css">
		body > div {
			width: 133px;
			height: 133px;
			border-radius: 31px;
			border: 3px solid #ebd5d5;
			position: fixed;
		}

		#mainButton {

		}
	</style>
</head>
<body>
	<script src="god.js"></script>
	<div id="life"></div>
	<div id="work"></div>
	<div id="joy"></div>
	<div id="mainButton"></div>
	<script type="text/javascript">
		function distance(p, pp) {
			return Math.sqrt(Math.pow(p.pageX - pp.pageX, 2) + Math.pow(p.pageY - pp.pageY, 2));
		}
		var main;
		function start(e) {
			if (main) {
				return;
			}
			main = {};
			var t0 = e.targetTouches[0];
			for (var i in t0) {
				main[i] = t0[i];
			}
			mainButton.addEventListener("touchmove", move);
		}
		function move(e) {
			var t0 = e.targetTouches[0];
			if (distance(main, t0) > 33) {
				document.title = "hahwlerh";
			}
		}
		god.window.dragable(mainButton, mainButton);
		ele.style.width = (window.outerWidth - ele.clientWidth) / 2 + "px";
		ele.style.height = (window.outerHeight - ele.clientHeight) / 2 + "px";
		//mainButton.addEventListener("touchstart", start);
	</script>
</body>
</html>
