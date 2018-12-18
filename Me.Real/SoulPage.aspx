<%@ Page Title="" Language="C#" MasterPageFile="~/Outside.Master" AutoEventWireup="true" CodeBehind="SoulPage.aspx.cs" Inherits="Me.Real.SoulPage" %>

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
	<div data-role="dialog" id="tagsDialog" data-overlay-click-close="true">
		<div class="dialog-title">选择标签</div>
		<div class="dialog-content">
			<ul class="listview view-icons" data-bind="foreach: Tags">
				<li class="node" data-bind="css: { 'current-select': $root.CurrentTag().Id === Id }, click: function () { return $root.CurrentTag() === $data ? resetCurrentTag() : $root.CurrentTag({ Name: Name, Id: Id }); }">
					<span class="icon">
						<img src="#" data-bind="attr: { src: `images/${Id}.png` }" /></span>
					<div class="data">
						<div class="caption" data-bind="text: Name"></div>
					</div>
				</li>
			</ul>
		</div>
		<div class="dialog-actions">
			<input type="button" class="button warning" value="编辑" data-bind="
	enable: ss.UpldateTag,
	value: CurrentTag().Id ? '编辑' : '新建',
	click: function () { Metro.dialog.open(newTagDialog); }
	" />
			<input type="button" class="button primary" value="确定" onclick="bindSelectedTag(1);" />
			<input type="button" class="button js-dialog-close" value="关闭" onclick="bindSelectedTag();" />
		</div>
	</div>
	<form data-role="dialog" id="newTagDialog" data-bind="with: CurrentTag">
		<input type="hidden" name="id" data-bind="value: Id" />
		<input type="file" name="image" style="display: none;" id="image" />
		<div class="dialog-title" data-bind="text: Id ? '编辑标签' : '新建标签'"></div>
		<div class="dialog-content">
			<div class="input">
				<div class="prepend">名称：</div>
				<input type="text" name="name" data-bind="value: Name" required />
				<div class="button-group">
					<button class="button input-custom-button success" type="button" onclick="image.click();">选择图标</button>
				</div>
			</div>
		</div>
		<div class="dialog-actions">
			<input type="button" class="button success" value="确定" onclick="s.UpdateTag(this);" />
			<input type="button" class="button warning" value="删除" data-bind="visible: Id" onclick="	s.DeleteTag(this);" />
			<input type="button" class="button js-dialog-close" value="关闭" />
		</div>
	</form>
	<script type="text/javascript">
		var vm = {
			Name: ko.observable(),
			selectedTag: ko.observable(),
			LastPlan: ko.observable(),
			LastEffort: ko.observable(),
			Plans: ko.observableArray(),
			Tags: ko.observableArray(),
			CurrentTag: ko.observable()
		};
		function resetCurrentTag() {
			vm.CurrentTag({ Name: "", Id: 0 });
		}
		resetCurrentTag();
		var s = new CSharp.Me.Inside.Soul();
		for (var i in s.progress) {
			//TODO:
		}
		var ss = Javascript.Me.Inside.Soul;
		function rebindTags() {
			resetCurrentTag();
			$(newTagDialog).data("dialog").close();
			s.Tags(vm.Tags);
		}
		rebindTags();
		ss.UpdateTag = u=> vm.Tags().some(e=>e.Name === vm.CurrentTag().Name);
		ss.UpdateTag = rebindTags;
		ss.DeleteTag = () =>!vm.CurrentTag();
		ss.DeleteTag = rebindTags;
		ko.applyBindings(vm);
		function bindSelectedTag(b) {
			vm.selectedTag(b ? vm.CurrentTag().Name : null);
			resetCurrentTag();
			$(tagsDialog).data("dialog").close();
		}
	</script>
</asp:Content>
