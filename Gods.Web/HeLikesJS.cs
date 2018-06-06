using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Routing;

namespace Gods.Web {
	public partial class Him {
		private static readonly JObject CSharp = new JObject();
		private static string webRoot;
		internal static string AjaxKey = "Him1344150689";//nameof(Him) + nameof(Him).GetHashCode()
		internal static string AjaxRoute = "him";
		public static bool AllowCache = true;

		public static string LoadJavascript(string name) {
			var v = CSharp.GetValue(name)?.ToString();
			return v == null ? null : Properties.Resources.Map
					.Replace(nameof(Route), '"' + AjaxRoute + '"')
					.Replace(nameof(AjaxKey), '"' + AjaxKey + '"')
					.Replace(nameof(CSharp), v);
		}

		public static string Modules = "bin";
		public static void Create<T>(string route) {
			RouteTable.Routes.Add(new Route(AjaxRoute = route, new Me()));
			TagInterface = typeof(T);
			webRoot = HostingEnvironment.MapPath("/");
			Directory.CreateDirectory(webRoot + "/Scripts/" + route);
			SetValidator(new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly);
			Directory.GetFiles(webRoot + Modules, "*.dll")
				.Select(e => Gods.Him.TryGet(() => Assembly.LoadFrom(e))).ToList()
				.ForEach(a => {
					var c = a?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;
					if (c.Length == 0 || c.StartsWith(nameof(Microsoft)) || c.StartsWith(nameof(Gods)) || c.StartsWith(nameof(Newtonsoft))) {
						return;
					}
					foreach (var t in a.ExportedTypes) {
						if (t.IsInterface && t.GetInterfaces().Any(i => i == TagInterface)) {
							Append(CSharp, t);
							cache.Add(new TypeCache(t));
						}
					}
				});
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
			var Methods = new JArray();
			item.GetMethods().Where(m => !m.IsSpecialName).ToList().ForEach(m => {
				var t = new JObject {
					[nameof(m.Name)] = m.Name,
					["Key"] = item.GetHashCode() + "." + Gods.Him.SignMethod(m)
				};
				var Parameters = JArray.FromObject(m.GetParameters().Select(e => e.Name).ToList());
				if (Parameters.Count > 0) {
					t[nameof(Parameters)] = Parameters;
				}
				var rt = m.ReturnType;
				JToken v;
				if (rt == typeof(string)) {
					v = string.Empty;
				} else if (rt == typeof(DateTime)) {
					v = DateTime.Now;
				} else if (rt.IsValueType) {
					v = 0;
				} else if (rt.GetInterfaces().Any(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
					rt = rt.IsArray ? rt.GetElementType() : rt.GenericTypeArguments[0];
					v = JArray.FromObject(new[] { Activator.CreateInstance(rt) });
				} else if (rt != typeof(void)) {
					v = JToken.FromObject(Activator.CreateInstance(rt));
				} else {
					v = null;
				}
				t["Return"] = v;
				Methods.Add(t);
			});
			if (Methods.Count > 0) {
				j[nameof(Methods)] = Methods;
			}
		}

		public static Type TagInterface { get; private set; }

		private static List<TypeCache> cache = new List<TypeCache>();
		private class TypeCache {
			public Type d;
			public Type i;

			public TypeCache(Type declaration) {
				d = declaration;
			}
			public Type GetImplement() {
				return i ?? (i = Gods.Him.FindInstance(d, Implements)?.GetType()) ?? d;
			}
		}
	}
}
