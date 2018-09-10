<%@ Page Language="C#" AutoEventWireup="true" Inherits="Gods.Web.Manage.Him" CodeBehind="~/Him.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title></title>
</head>
<body>
	<table>
		<thead>
			<tr>
				<th>名称</th>
				<th>描述</th>
				<th>当前负责人</th>
				<th>上次负责人</th>
			</tr>
		</thead>
		<asp:Repeater runat="server" ID="eee">
			<ItemTemplate>
				<tr>
					<td><%#Eval("Name") %></td>
					<td><%#Eval("Description") %></td>
					<td><%#Eval("CoderName") %></td>
					<td><%#Eval("RecordsCount") %></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>
</body>
</html>
