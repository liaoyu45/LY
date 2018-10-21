<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Manage.Him" CodeBehind="~/Him.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title></title>
</head>
<body>
	<form runat="server" enableviewstate="false">
		<asp:Repeater runat="server" ID="interfaces">
			<ItemTemplate>
				<dl>
					<dt><%#Container.DataItem %></dt>
					<asp:Repeater runat="server">
						<ItemTemplate>
							<dd><%#Container.DataItem %></dd>
						</ItemTemplate>
					</asp:Repeater>
				</dl>
			</ItemTemplate>
		</asp:Repeater>
	</form>
</body>
</html>
