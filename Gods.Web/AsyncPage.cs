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
		public AsyncPage() {
			var r = new List<MethodInfo>();
			LoadDllInBin(BllPath, a => {
				if (a.FullName == WebApp.FullName) {
					return;
				}
				var c = a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
				if (c.StartsWith(nameof(Microsoft)) || c.StartsWith(nameof(Gods)) || c.StartsWith(nameof(Newtonsoft))) {
					return;
				}
				r.AddRange(a.ExportedTypes.Where(FilterTypes).SelectMany(e => e.GetMethods().Where(ee => FilterMethods(e, ee))));
			});
			methods = r.ToArray();
		}

		protected virtual bool FilterTypes(Type type) {
			return !type.IsInterface;
		}

		protected virtual bool FilterMethods(Type declare, MethodInfo method) {
			return method.DeclaringType == declare && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_");
		}
		internal static Assembly WebApp;
		internal static void LoadDllInBin(string path, Action<Assembly> dowith) {
			var fs = Directory.GetFiles(path, "*.dll");
			foreach (var item in fs) {
				try {
					var a = Assembly.LoadFrom(item);
					dowith(a);
				} catch {
				}
			}
		}
		private static Dictionary<Guid, Dictionary<string, MethodInfo>> cache = new Dictionary<Guid, Dictionary<string, MethodInfo>>();
		protected string BllPath { get; } = HostingEnvironment.ApplicationPhysicalPath + "/bin";
		private MethodInfo[] methods;

		public virtual string AjaxKey { get; } =
			Guid.NewGuid().ToString().Replace("-", "");

		public virtual string ThisArg { get; } = "window";

		public override void ProcessRequest(HttpContext context) {
			var r = HttpContext.Current.Request;
			if (!r.Params.AllKeys.Contains(AjaxKey)) {
				base.ProcessRequest(context);
				return;
			}
			var result = Invoke() ?? string.Empty;
			if (result is string) {
			} else if (result.GetType().IsClass) {
				context.Response.ContentType = "application/json";
				result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
			} else {
				if (result is Guid && ((Guid)result == GetType().GUID)) {
					result = "not found";
				}
			}
			context.Response.Write(result);
		}

		private object Invoke() {
			var m = default(MethodInfo);
			var t = GetType().GUID;
			if ((cache.ContainsKey(t) ? cache[t] : cache[t] = methods.ToDictionary(mm => mm.GetHashCode().ToString())).TryGetValue(HttpContext.Current.Request[AjaxKey], out m)) {
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
		public static void MakeScripts(string relativePath) {
			WebApp = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly;
			var folder = HostingEnvironment.MapPath("/" + relativePath);
			if (Directory.Exists(folder)) {
				Directory.Delete(folder, true);
			}
			Directory.CreateDirectory(folder);
			var all = WebApp.DefinedTypes.Where(t => t.IsSubclassOf(typeof(AsyncPage))).Select(t => Activator.CreateInstance(t)).Cast<AsyncPage>().ToList();
			if (all.Count == 0) {
				all.Add(new AsyncPage());
			}
			foreach (var item in all) {
				var CSharp = new JObject();
				item.methods.Select(e => e.DeclaringType).Distinct().ToList().ForEach(t => Append(CSharp, t));
				File.WriteAllText($"{folder}/{item.GetType().Name}.js",
					Properties.Resources.Map
					.Replace(nameof(ThisArg), item.ThisArg)
					.Replace(nameof(AjaxKey), item.AjaxKey)
					.Replace(nameof(CSharp), CSharp.ToString()));
			}
		}

		private static void Append(JObject j, Type item) {
			var ns = item.FullName.Split('.');
			foreach (var n in ns) {
				if (j.Properties().Any(p => p.Name == n)) {
					j = j[n] as JObject;
				} else {
					j = (j[n] = new JObject()) as JObject;
				}
			}
			var Properties = GetProperties(item);
			if (Properties.Count > 0) {
				j[nameof(Properties)] = Properties;
			}
			var Methods = new JArray();
			item.GetMethods().Where(e => e.DeclaringType == item && !e.Name.StartsWith("get_") && !e.Name.StartsWith("set_")).ToList().ForEach(m => {
				var t = new JObject {
					[nameof(m.Name)] = m.Name,
					["Key"] = m.GetHashCode()
				};
				var Parameters = JArray.FromObject(m.GetParameters().Select(e => new {
					e.Name, Type = e.ParameterType.FullName
				}));
				if (Parameters.Count > 0) {
					t[nameof(Parameters)] = Parameters;
				}
				if (!m.ReturnType.IsValueType && m.ReturnType != typeof(string) && m.ReturnType != typeof(void)) {
					t[nameof(MethodInfo.ReturnType)] = GetProperties(m.ReturnType);//TODO:把整个返回类型的结构传到JS。
				}
				Methods.Add(t);
			});
			if (Methods.Count > 0) {
				j[nameof(Methods)] = Methods;
			}
		}

		private static JArray GetProperties(Type item) {
			return JArray.FromObject(item.GetProperties().Where(p => p.DeclaringType == item).Select(e => new {
				e.Name, Type = e.PropertyType.FullName
			}));
		}

	}
}