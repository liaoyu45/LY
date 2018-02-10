using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace AjaxPage {
	public abstract class PageBase : Page {
		protected virtual string ValidatorNamespace => GetType().Namespace.Split('.')[0];
		protected virtual string Action { get; } = nameof(Action).ToLower();
		public override void ProcessRequest(HttpContext context) {
			if (context.Request.HttpMethod == "get" && context.Request.QueryString.Count == 0) {
				base.ProcessRequest(context);
				return;
			}
			bool ok;
			var result = getResult(out ok);
			context.Response.Write(result);
		}

		private object getResult(out bool ok) {
			ok = false;
			var action = HttpContext.Current.Request["action"];
			if (action?.Any() != true) {
				return "action empty";
			}
			var t = Type.GetType(action.Split('/')[0], false, false);
			const BindingFlags f = (BindingFlags)63;
			var mthd = t?.GetMethods(f).FirstOrDefault(m => m.Name == action.Split('/')[1] && m.GetParameters().Length == 0);
			if (mthd == null) {
				return "action not found";
			}
			var tar = getTar(t);
			object result;
			var bt = t.BaseType;
			while (bt.BaseType != typeof(object)) {
				bt = bt.BaseType;
			}
			var vt = Type.GetType(ValidatorNamespace + '.' + bt.Name, false, false);
			if (vt != null) {
				var cs = vt.GetConstructors(f).OrderByDescending(c => c.GetParameters().Length).First();
				var ps = cs.GetParameters();
				var paras = ps.Length == 0 ? Type.EmptyTypes : ps[0].ParameterType == typeof(object) ? new[] { tar } : null;
				if (paras == null) {
					return "bad validator";
				}
				try {
					if (null != (result = vt.GetMethod(mthd.Name)?.Invoke(cs.Invoke(paras), Type.EmptyTypes))) {
						return result;
					}
				} catch (Exception e) {
					return e.Message;
				}
			}
			try {
				if (null != (result = mthd.Invoke(tar, Type.EmptyTypes))) {
					ok = true;
					return result;
				}
			} catch (Exception e) {
				return e;
			}
			return null;
		}

		private object getTar(Type t) {
			var @params = HttpContext.Current.Request.Params;
			var tar = Activator.CreateInstance(t);
			var files = HttpContext.Current.Request.Files;
			var alps = t.GetProperties().Where(p => p.CanWrite);
			alps.Where(p => p.PropertyType.IsValueType).ToList().ForEach(p => {
				var ops = new object[] { @params[p.Name], null };
				var rp = p.PropertyType.IsGenericType ? p.PropertyType.GenericTypeArguments[0] : p.PropertyType;
				var ok = (bool)rp.GetMethod("TryParse", new[] { typeof(string), rp }).Invoke(null, ops);
				if (ok) {
					p.SetValue(tar, ops[1]);
				}
			});
			alps.Where(p => p.PropertyType == typeof(string)).ToList().ForEach(p => {
				p.SetValue(tar, @params[p.Name]);
			});
			alps.Where(p => p.PropertyType == typeof(byte[])).ToList().ForEach(p => {
				byte[] bs;
				if (files.AllKeys.Contains(p.Name)) {
					bs = new byte[files[p.Name].InputStream.Length];
					files[p.Name].InputStream.Read(bs, 0, bs.Length);
				} else {
					try {
						bs = Convert.FromBase64String(@params[p.Name]);
					} catch {
					}
				}
			});
			foreach (var item in alps) {
				var i = item.PropertyType.GetInterface("IEnumerable`1")?.GenericTypeArguments[0];
				if (i != typeof(string) || i?.IsValueType != true) {
					continue;
				}
				var arr = @params[item.Name]?.Split(',');
				if (i == typeof(string)) {
					item.SetValue(tar, arr);
				}
			}
			return tar;
		}
		private object parsePara(ParameterInfo p) {
			var t = p.ParameterType;
			var n = p.Name;
			return getV(t, n);
		}

		private static object getV(Type t, string n) {
			var r = HttpContext.Current.Request;
			var tt = t;
			if (tt.IsValueType) {
				if (tt.Name == "Nullable`1") {
					tt = tt.GenericTypeArguments[0];
				}
				var parameters = new object[] { r[n], null };
				tt.GetMethod("TryParse", new[] { typeof(string), tt }).Invoke(null, parameters);
				return parameters[1];
			}
			if (tt == typeof(string)) {
				return r[n];
			}
			if (tt.GetInterface("IEnumerable`1") == null) {
			}
			tt = tt.GenericTypeArguments[0];
			r[n].Split(',').Select(s => {
				var ps = new object[] { s, null };
				tt.GetMethod("TryParse", new Type[] { typeof(string), tt }).Invoke(null, ps);
				return ps[1];
			});
			return 1;
		}
	}
}
