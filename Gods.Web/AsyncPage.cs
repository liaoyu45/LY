using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace Gods.Web {
	public class Me : You, IRequiresSessionState { }

	public class You : Page, IHttpHandler {
		public virtual ICacheManager CacheManager => Web.CacheManager.Instance;
		public virtual IValidator Validator => Web.Validator.Instance;

		public override void ProcessRequest(HttpContext context) {
			if (!Regex.IsMatch(context.Request[Him.AjaxKey]?.Trim() ?? string.Empty, @"^\d+\.\d+$")) {
				base.ProcessRequest(context);
				return;
			}
			var result = Gods.Him.TryGet(Invoke, string.Empty);
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
			var re = HttpContext.Current.Request;
			var args = re[Him.AjaxKey].Split('.');
			var t = Him.FindImplement(args[0]);
			var m = Gods.Him.GetSignedMethod(t, int.Parse(args[1]), nameof(Gods));
			object r;
			var a = CacheManager?.Cacheable(m) ?? 0;
			if (a > 0) {
				r = CacheManager.Read(a);
				if (r != null) {
					return r;
				}
			}
			var ins = Activator.CreateInstance(t);
			if (ins is AOP.Model) {
				var model = ins as AOP.Model;
				foreach (var item in re.Params.AllKeys) {
					model.Values[item] = re[item];
				}
				foreach (var item in re.Files.AllKeys) {
					model.Values[item] = re.Files[item];
				}
				r = AOP.Mapper.Invoke(model, m);
			} else {
				Mapper.MapProperties(ins);
				var ps = Mapper.MapParameters(m);
				ps = Validator?.Validate(ins, m, ps) as object[] ?? ps;
				r = m.Invoke(ins, ps);
			}
			if (a > 0) {
				CacheManager.Save(a, r);
			} else {
				CacheManager?.Remove(a);
			}
			return r;
		}
	}
}