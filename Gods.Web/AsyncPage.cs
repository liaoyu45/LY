using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.UI;

namespace Gods.Web {
	public class Me : You, IRequiresSessionState, IRouteHandler {
		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) => this;
	}

	public partial class You : Page, IHttpHandler {
		protected override HttpContext Context => HttpContext.Current;

		public override void ProcessRequest(HttpContext context) {
			var key = context.Request[Him.AjaxKey]?.Trim() ?? string.Empty;
			var action =
				GetType() == typeof(Me) && key.Length == 0 ? nameof(WriteJs) :
				Regex.IsMatch(key, @"^-?\d+\.-?\d+$") ? nameof(MatchHashCode) :
				Regex.IsMatch(key, "[a-zA-Z_][a-zA-Z_0-9]*") ? nameof(MatchThis) : nameof(ProcessRequest);
			typeof(You).GetMethod(action, (BindingFlags)36)?.Invoke(this, new[] { key });
		}

		private void WriteJs(string key) {
			var n = (Context.Request.Url.Query.Split('&', '?', '=').FirstOrDefault(e => e.Any()) ?? string.Empty).Trim();
			if (n.Length == 0) {
				return;
			}
			var path = Context.Request.MapPath($"/Scripts/{Him.AjaxRoute}/{n}.js");
			if (System.IO.File.Exists(path)) {
				if (System.IO.File.ReadLines(path).FirstOrDefault().Split('/').LastOrDefault() == Him.CSharp.GetHashCode().ToString()) {
					return;
				}
			}
			var v = Him.LoadJavascript(n);
			if (v != null) {
				Context.Response.Write(v);
				if (Him.AllowCache) {
					System.IO.File.WriteAllText(path, v);
				}
			}
		}

		private void ProcessRequest(string key) {
			base.ProcessRequest(Context);
		}

		private void MatchThis(string key) {
			var t = GetType();
			t.GetMethods().FirstOrDefault(e => e.DeclaringType == t && e.Name == key)?.Invoke(this, null);
		}

		private void MatchHashCode(string key) {
			var args = key.Split('.').Select(int.Parse).ToArray();
			var bad = GetType().GUID;
			var result = Gods.Him.TryGet(() => Him.Invoke(args[0], args[1], s => Context.Request[s]), bad);
			if (result is string) {
				Context.Response.StatusCode = 501;
				return;
			} else if (result.GetType().IsClass) {
				Context.Response.ContentType = "application/json";
				result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
			} else if (result is Guid && (Guid)result == bad) {
				Context.Response.StatusCode = 404;
				return;
			}
			Context.Response.Write(result);
		}
	}
}