using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Gods.Web {
	public abstract class AjaxPage : Page {

		public AjaxPage() : this("ajax") { }

		public AjaxPage(string ajax) : base() {
			ajaxKey = ajax;
			Load += (s, e) => {
				if (IsPostBack) {
					return;
				}
				try {
					if (Initiate()) {
						return;
					}
				} catch {
				}
				OnError(EventArgs.Empty);

			};
		}

		protected abstract Func<object, string> Serializer { get; }

		private string ajaxKey;

		protected virtual bool Initiate() => true;

		public override void ProcessRequest(HttpContext context) {
			if (string.IsNullOrWhiteSpace(ajaxKey) || string.IsNullOrWhiteSpace(context.Request[ajaxKey])) {
				base.ProcessRequest(context);
				return;
			}
			bool ok;
			var result = tryAjax(out ok);
			try {
				var json = Serializer?.Invoke(new { ok, result });
				if (string.IsNullOrWhiteSpace(json)) {
					return;
				}
				context.Response.ContentType = "application/json";
				context.Response.Write(json);
			} catch {
			}
		}

		private object tryAjax(out bool ok) {
			var context = HttpContext.Current;
			var name = context.Request[ajaxKey].Trim();
			var m = GetType().BaseType.GetMethods((System.Reflection.BindingFlags)63).FirstOrDefault(mi => mi.Name == name);
			if (m == null || m.GetParameters().Length > 0) {
				ok = false;
				return $@"Valid server method ""{name}"" does not exist.";
			}
			var vs = m.GetCustomAttributes(typeof(HttpValidationAttribute), true) as HttpValidationAttribute[];
			foreach (var v in vs) {
				try {
					var r = v.Validate();
					if (r != null) {
						ok = false;
						return r;
					}
				} catch (Exception e) {
					ok = false;
					return e.Message;
				}
			}
			try {
				ok = true;
				return m.Invoke(this, null);
			} catch (Exception e) {
				ok = false;
				return e.Message;
			}
		}
	}
	public abstract class HttpValidationAttribute : Attribute {
		public abstract string Validate();
	}
}
