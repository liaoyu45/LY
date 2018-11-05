<%@ Page Title="" Language="C#" MasterPageFile="~/Outside.Master" AutoEventWireup="true" CodeBehind="Soul.aspx.cs" Inherits="Me.Real.Soul" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="http://localhost:54734/CSharp?Me.js"></script>
	<style type="text/css">
		input[type=date] {
			border: none;
			background-color: transparent;
			color: white;
		}

		.frame span {
			display: none;
		}

		.frame.active span {
			display: inline;
		}
	</style>
	<script type="text/javascript">
		var saveNewPlan = [{
			html: "<span class='mif-play'></span>",
			cls: "alert",
			onclick: "alert('You press user button')"
		}];
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1 data-bind="text: Name" class="mif-"></h1>
	<div data-role="accordion" data-active-heading-class="bg-cyan fg-white" data-active-content-class="bg-dark fg-white">
		<div class="frame">
			<div class="heading">全新的计划</div>
			<form class="content">
				<div>
					<textarea name="plan" placeholder="内容" class="h-100"></textarea>
				</div>
				<div class="input">
					<div class="prepend">标签：</div>
					<input type="text" data-bind="value: selectedTag" readonly onclick="	Metro.dialog.open(tagsDialog);" />
					<div class="button-group">
						<button class="button input-custom-button success" type="button"><span class="mif-play"></span></button>
					</div>
				</div>
			</form>
		</div>
		<div class="frame">
			<div class="heading">最近的努力：<span data-bind="text: 'Content'"></span></div>
			<div class="content" data-bind="with: LastEffort">
				<div data-bind="foreach: Efforts">
				</div>
			</div>
		</div>
		<div class="frame">
			<div class="heading">最近的计划：<span data-bind="text: 'Content'"></span></div>
			<div class="content" data-bind="with: LastPlan">
				<div data-bind="foreach: Efforts"></div>
			</div>
		</div>
		<div class="frame">
			<div class="heading">
				查询：<span><input type="date" name="start" />~<input type="date" name="end" /></span>
			</div>
			<div class="content">
				<form>
					<select data-role="select">
						<optgroup label="Physical servers">
							<option value="dedicated_corei3_hp">Core i3 (hp)</option>
							<option value="dedicated_pentium_hp">Pentium (hp)</option>
							<option value="dedicated_smart_corei3_hp">Smart Core i3 (hp)</option>
						</optgroup>
						<optgroup label="Virtual hosting">
							<option value="mini">Mini</option>
							<option value="site">Site</option>
							<option value="portal">Portal</option>
						</optgroup>
						<optgroup label="Virtual servers">
							<option value="evps0">eVPS-TEST (30 дней)</option>
							<option value="evps1" selected="selected">eVPS-1</option>
							<option value="evps2">eVPS-2</option>
						</optgroup>
					</select>
				</form>
				<div data-bind="foreach: Plans"></div>
			</div>
		</div>
	</div>
	<script type="text/javascript">
		var saveNewPlan = {
			cls: "button",
			html: "保存",
			onclick: () =>alert(1)
		};
		var vm = {};

	</script>
	<div data-role="dialog" id="tagsDialog" data-overlay-click-close="true">
		<div class="dialog-title">选择标签</div>
		<div class="dialog-content">
			<ul data-role="listview" data-view="icons" id="tags" data-on-node-click="vm.selectedTag($(this).data('listview').getSelected)">
				<li data-icon="<span class='mif-folder fg-orange'>"
					data-caption="Video"
					data-content="<div class='mt-1' data-role='progress' data-value='35' data-small='true'>"></li>
				<li data-icon="<span class='mif-folder fg-cyan'>"
					data-caption="Images"
					data-content="<div class='mt-1' data-role='progress' data-value='78' data-small='true'>"></li>
			</ul>
		</div>
		<div class="dialog-actions">
			<form class="input">
				<input type="text" />
				<div class="button-group">
					<input type="button" class="button success" value="新建" />
					<input type="button" class="button" value="取消" />
					<input type="button" class="button primary" value="确定" />
				</div>
			</form>
		</div>
	</div>
	<script type="text/javascript">
		var vm = {
			Name: ko.observable(),
			selectedTag: ko.observable(),
			LastPlan: ko.observable(),
			LastEffort: ko.observable(),
			Plans: ko.observableArray()
		};
		ko.applyBindings(vm);
		function bindSelectedTag(v) {
			vm.selectedTag(v);
		}
	</script>
</asp:Content>
