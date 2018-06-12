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
			var key = context.Request[Him.his.AjaxKey]?.Trim() ?? string.Empty;
			var action =
				GetType() == typeof(Me) && key.Length == 0 ? nameof(WriteJs) :
				Regex.IsMatch(key, @"^-?\d+\.-?\d+$") ? nameof(MatchHashCode) :
				Regex.IsMatch(key, "[a-zA-Z_][a-zA-Z_0-9]*") ? nameof(MatchThis) : nameof(ProcessRequest);
			object result;
			try {
				result = typeof(You).GetMethods((BindingFlags)36).FirstOrDefault(e => e.Name == action)?.Invoke(this, new[] { key });
			} catch {
				result = typeof(You).GUID;
				context.Response.StatusCode = 404;
			}
			if (result != null && result.GetType() != typeof(string) && result.GetType().IsClass) {
				context.Response.ContentType = "application/json";
				result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
			}
			context.Response.Write(result);
		}

		private void WriteJs(string key) {
			var n = (Context.Request.Url.Query.Split('&', '?', '=').FirstOrDefault(e => e.Any()) ?? string.Empty).Trim();
			if (n.Length == 0) {
				return;
			}
			var path = Context.Request.MapPath($"/Scripts/{Him.his.AjaxRoute}/{n}.js");
			if (System.IO.File.Exists(path)) {
				if (System.IO.File.ReadLines(path).FirstOrDefault().Split('/').LastOrDefault() == Him.HashCode.ToString()) {
					return;
				}
			}
			var v = Him.LoadJavascript(n);
			if (v != null) {
				Context.Response.Write(v);
				if (Him.his.AllowCache) {
					System.IO.File.WriteAllText(path, v);
				}
			}
		}

		private void ProcessRequest(string key) {
			base.ProcessRequest(Context);
		}

		private object MatchThis(string key) {
			var t = GetType();
			return t.GetMethods().FirstOrDefault(e => e.DeclaringType == t && e.Name == key)?.Invoke(this, null);
		}

		private object MatchHashCode(string key) {
			var args = key.Split('.').Select(int.Parse).ToArray();
			return Him.Invoke(args[0], args[1], s => Context.Request[s]);
		}
	}
}