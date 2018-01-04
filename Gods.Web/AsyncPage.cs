using Javascript.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace Gods.Web {
	public class AsyncPage : System.Web.UI.Page {
		internal static Dictionary<Guid, Dictionary<string, MethodInfo>> cache = new Dictionary<Guid, Dictionary<string, MethodInfo>>();
		internal static Dictionary<Guid, string> keys = new Dictionary<Guid, string>();

		internal static Assembly WebApp;
		internal static string AjaxKey;
		public override void ProcessRequest(HttpContext context) {
			var r = HttpContext.Current.Request;
			if (!r.Params.AllKeys.Contains(AjaxKey)) {
				base.ProcessRequest(context);
				return;
			}
			var result = Invoke() ?? string.Empty;
			if (result is string) {
				context.Response.StatusCode = 501;
			} else if (result.GetType().IsClass) {
				context.Response.ContentType = "application/json";
				result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
			} else {
				if (result is Guid && ((Guid)result == GetType().BaseType.GUID)) {
					context.Response.StatusCode = 404;
					result = string.Empty;
				}
			}
			context.Response.Write(result);
		}

		private object Invoke() {
			var m = default(MethodInfo);
			var t = GetType().BaseType.GUID;
			if (cache[t].TryGetValue(HttpContext.Current.Request[AjaxKey], out m)) {
				var tar = JSON.From(m.DeclaringType.AssemblyQualifiedName).ForIn(p => p.PropertyType.map(p.Name));
				try {
					return tar[m](GetArgs(m, tar.Source));
				} catch (Exception e) {
					return e.Final().Message;
				}
			}
			return t;
		}
		private object[] GetArgs(MethodInfo m, object tar) {
			var vt = LoadValidator(m);
			if (vt != null) {
				var c = vt.GetConstructors()[0];
				var ca = c.GetParameters().Length == 0 ? null : new[] { tar };
				var v = JSON.From(c.Invoke(ca))[mm => mm.Name == m.Name];
				if (v != null) {
					var args = v?.Invoke(null);
					if (args != null) {
						return args is object[] ? args as object[] : new[] { args };
					}
				}
			}
			return m.GetParameters().Select(p => p.ParameterType.map(p.Name)).ToArray();
		}

		protected virtual Type LoadValidator(MethodInfo m) {
			return WebApp.GetType(WebApp.FullName.Split(',')[0] + '.' + m.DeclaringType.FullName);
		}

		protected internal virtual bool FilterMethod(MethodInfo method) {
			return method.DeclaringType != typeof(object);
		}

		protected internal virtual bool FilterType(Type type) {
			return true;
		}

		protected internal virtual bool FilterAssembly(Assembly assembly) {
			return true;
		}
	}
}