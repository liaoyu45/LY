<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="AllOfMe.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form>
		<div>
			<input type="text" name="Name" value="109238" />
			<input type="text" name="Intro" value="109238" />
			<input type="text" name="[Items]Words" value="qwoieuro" />
			<input type="text" name="[Items]Tag" value="9123" />
			<input type="file" name="qwer" value="" />
			<input type="button" name="name" value="10928310" onclick="ssss()" />
		</div>
	</form>
	<script type="text/javascript">
		function ssss() {
			var f = objectToFormData({ a: 1, b: [{ e: 1 }, { e: 2 }]}, null);
			var r = new XMLHttpRequest();
			r.open("post", "?a=s");
			r.send(f);
		}
		function objectToFormData(obj, form, namespace) {
			const fd = form || new FormData();
			let formKey;

			for (var property in obj) {
				if (obj.hasOwnProperty(property)) {
					let key = Array.isArray(obj) ? '[]' : `[${property}]`;
					if (namespace) {
						formKey = namespace + key;
					} else {
						formKey = property;
					}
					// if the property is an object, but not a File, use recursivity.
					if (typeof obj[property] === 'object' && !(obj[property] instanceof File)) {
						objectToFormData(obj[property], fd, formKey);
					} else {
						// if it's a string or a File object
						fd.append(formKey, obj[property]);
					}
				}
			}
			return fd;
		}
		objectToFormData({ a: 1 }, null, "a");
	</script>
</body>
</html>
