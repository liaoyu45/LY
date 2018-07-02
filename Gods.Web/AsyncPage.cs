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
		public IHttpHandler GetHttpHandler(RequestContext requestContext) => this;
	}

	public partial class You : Page, IHttpHandler {
		protected override HttpContext Context => HttpContext.Current;

		public override void ProcessRequest(HttpContext context) {
			var key = context.Request[Him.his.AjaxKey]?.Trim() ?? string.Empty;
			var action =
				Regex.IsMatch(key, @"^-?\d+\.-?\d+$") ? nameof(MatchHashCode) :
				Regex.IsMatch(key, "[a-zA-Z_][a-zA-Z_0-9]*") ? nameof(MatchThis) : nameof(ProcessRequest);
			object result;
			try {
				result = typeof(You).GetMethods((BindingFlags)36).FirstOrDefault(e => e.Name == action)?.Invoke(this, new[] { key });
			} catch (Exception e) {
				while (e.InnerException != null) {
					e = e.InnerException;
				}
				result = e.Message;
				context.Response.StatusCode = 404;
			}
			if (result != null) {
				if (result is DateTime) {
					result = ((DateTime)result).ToString("YYYY-MM-DD HH:ss:mm");
				} else if (result.GetType() != typeof(string) && result.GetType().IsClass) {
					context.Response.ContentType = "application/json";
					result = Newtonsoft.Json.JsonConvert.SerializeObject(result, new Newtonsoft.Json.JsonSerializerSettings {
						ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
					});
				}
			}
			context.Response.Write(result);
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
			return Him.Invoke(args[0], args[1]);
		}
	}
}