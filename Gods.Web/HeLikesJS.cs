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
		internal static readonly JObject CSharp = new JObject();
		private static string webRoot;
		public static string AjaxKey { get; private set; } = "Him1344150689";//nameof(Him) + nameof(Him).GetHashCode()
		public static string AjaxRoute { get; private set; } = "Gods";
		public static bool AllowCache { get; set; } = true;

		public static string LoadJavascript(string name) {
			return Properties.Resources.Map1.Replace(nameof(AjaxRoute), AjaxRoute).Replace(nameof(AjaxKey), AjaxKey).Replace("Value", CSharp.GetValue(name)?.ToString())
				.Replace(nameof(GetHashCode), CSharp.GetHashCode().ToString())
				.Replace("Filename", name);
		}

		public static string Modules { get; set; } = "bin";
		public static void Create<T>() {
			Create<T>(AjaxRoute, AjaxKey);
		}
		public static void Create<T>(string route, string ajaxKey) {
			var t = typeof(T);
			if (!t.IsInterface) {
				throw new ArgumentException("T should be an interface.");
			}
			const string pattern = "[a-zA-Z_][a-zA-Z_0-9]*";
			if (!System.Text.RegularExpressions.Regex.IsMatch(route, pattern)) {
				throw new ArgumentException(nameof(route) + " should match " + pattern);
			}
			AjaxKey = ajaxKey;
			RouteTable.Routes.Add(new Route(AjaxRoute = route, new Me()));
			if (t.IsGenericType) {
				if (t.GenericTypeArguments.Length > 1 || !t.GenericTypeArguments[0].IsInterface) {
					throw new ArgumentException("If T is generic, T should have only one type argument 'X' and 'X' should be an interface too.");
				}
				validatorType = t.GetGenericTypeDefinition();
				TagInterface = t.GenericTypeArguments[0];
			} else {
				TagInterface = t;
				validatorType = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly.DefinedTypes.FirstOrDefault(e => e.IsGenericType && e.IsInterface && e.GenericTypeArguments[0].IsInterface);
			}
			webRoot = HostingEnvironment.MapPath("/");
			Directory.CreateDirectory(webRoot + "/Scripts/" + route);
			File.WriteAllText($"{webRoot}/Scripts/{route}/{route}.js", Properties.Resources.Map.Replace(nameof(Route), route));
			Directory.GetFiles(webRoot + Modules, "*.dll")
				.Select(e => Gods.Him.TryGet(() => Assembly.LoadFrom(e))).ToList()
				.ForEach(a => {
					var c = a?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;
					if (c.Length == 0 || c.StartsWith(nameof(Microsoft)) || c.StartsWith(nameof(Gods)) || c.StartsWith(nameof(Newtonsoft))) {
						return;
					}
					foreach (var e in a.ExportedTypes) {
						if (e.IsInterface && e.GetInterfaces().Any(i => i == TagInterface)) {
							Append(CSharp, e);
							cache.Add(new TypeCache(e));
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
					["Key"] = item.GetHashCode() + "." + Math.Abs(Gods.Him.SignMethod(m))
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
				} else {
					v = JToken.FromObject(Activator.CreateInstance(rt));
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
			public Type Declare { get; set; }
			public Type Implement { get; set; }

			public TypeCache(Type declaration) {
				Declare = declaration;
			}
			public Type GetImplement() {
				return Implement ?? (Implement = Gods.Him.FindInstance(Declare, webRoot + Implements)?.GetType()) ?? Declare;
			}
		}
	}
}
