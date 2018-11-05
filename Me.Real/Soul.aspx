<%@ Page Title="" Language="C#" MasterPageFile="~/Outside.Master" AutoEventWireup="true" CodeBehind="Soul.aspx.cs" Inherits="Me.Real.Soul" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		table {
			width: 100%;
		}

		tr td:first-child {
			text-align: right;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1 data-bind="text: Name"></h1>
		<form class="frame">
			<div class="heading">新计划</div>
			<div class="content">
				<textarea name="plan" data-role="textarea" placeholder="内容"></textarea>
				<input type="text" data-role="input" data-prepend="标签：" placeholder="woiruowiu">
				<fieldset>
					<legend>历史标签</legend>
					<div data-bind="foreach: tags">
						<input type="checkbox" name="tags" data-bind="checkedValue: $data" />
					</div>
				</fieldset>
			</div>
		</form>
	<div data-role="accordion"
		data-active-heading-class="bg-cyan fg-white"
		data-active-content-class="bg-dark fg-white">
		<div class="frame" data-bind="with: LastPlan">
			<div class="heading">最近的计划：<span data-bind="text: Content"></span></div>
			<div class="content">
				<div data-bind="foreach: Efforts">
				</div>
			</div>
		</div>
		<div class="frame" data-bind="with: LastEffort">
			<div class="heading">上一次努力：<span data-bind="text: Content"></span></div>
			<div class="content">
				<div data-bind="foreach: Efforts">
				</div>
			</div>
		</div>
		<div class="frame" data-bind="with: ForgottenPlan">
			<div class="heading">久远的计划：<span data-bind="text: Content"></span></div>
			<div class="content">
				<div data-bind="foreach: Efforts">
				</div>
			</div>
		</div>
	</div>
</asp:Content>
