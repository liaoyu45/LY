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
		private static readonly JObject Javascript = new JObject();
		private static Type tagInterface;
		private static Type validatorType;
		private static Dictionary<int, Func<object, MethodInfo, object[], object>> validators = new Dictionary<int, Func<object, MethodInfo, object[], object>>();
		private static List<TypeCache> cache = new List<TypeCache>();
		internal static His his;

		public static int HashCode => CSharp.GetHashCode();

		public static void Create<T>(His his) {
			if (!System.Text.RegularExpressions.Regex.IsMatch(his.AjaxRoute, "[a-zA-Z_][a-zA-Z_0-9]*")) {
				throw new ArgumentException("invalid " + nameof(His.AjaxRoute), nameof(his));
			}
			var t = typeof(T);
			if (!t.IsInterface) {
				throw new ArgumentException("T should be an interface.");
			}
			if (t.IsGenericType) {
				if (t.GenericTypeArguments.Length > 1 || !t.GenericTypeArguments[0].IsInterface) {
					throw new ArgumentException("If T is generic, T should have only one type argument 'X' and 'X' should be an interface too.");
				}
				validatorType = t.GetGenericTypeDefinition();
				tagInterface = t.GenericTypeArguments[0];
			} else {
				tagInterface = t;
				validatorType = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly.DefinedTypes.FirstOrDefault(e => e.IsGenericType && e.IsInterface && e.GenericTypeArguments[0].IsInterface);
			}
			Him.his = his;
			RouteTable.Routes.Add(new Route(his.AjaxRoute, new Me()));
			var webRoot = HostingEnvironment.MapPath("/");
			var c = $"{webRoot}/Scripts/{nameof(Gods)}";
			his.Implements = webRoot + his.Implements;
			his.Validators = webRoot + his.Validators;
			his.Modules = webRoot + his.Modules;
			Directory.GetFiles(his.Modules, "*.dll").SelectMany(a => {
				try {
					return Assembly.LoadFrom(a)?.ExportedTypes.Where(e => e.GetInterfaces().Contains(tagInterface) && !e.IsGenericType && (e.IsInterface || !e.IsAbstract && e.GetInterfaces().All(ee => !ee.GetInterfaces().Contains(tagInterface))));
				} catch {
					return Enumerable.Empty<Type>();
				}
			}).ToList().ForEach(Append);
			Directory.CreateDirectory(c);
			foreach (var item in CSharp) {
				File.WriteAllText($"{c}/{nameof(CSharp)}/{item.Key}.js", $"Him.{nameof(CSharp)} = " + item.Value.ToString(Newtonsoft.Json.Formatting.Indented));
			}
			foreach (var item in Javascript) {
				File.WriteAllText($"{c}/{nameof(Javascript)}/{item.Key}.js", $"window.{item.Key} = Him.{nameof(Javascript)} = " + item.Value.ToString(Newtonsoft.Json.Formatting.Indented));
			}
			File.WriteAllText($"{c}/{nameof(His)}.js", $@"
(function () {{
	function load() {{
		Him('{his.AjaxRoute}', '{his.AjaxKey}');
		removeEventListener('load', load);
	}}
	addEventListener('load', load);
}})();");
		}

		private static void Append(Type item) {
			var javascript = MapNamespace(item, Javascript);
			item.GetMethods().Where(m => !m.IsSpecialName).ToList().ForEach(m => javascript[m.Name] = null);
			var csharp = MapNamespace(item, CSharp);
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
				csharp[nameof(Methods)] = Methods;
			}
			cache.Add(new TypeCache(item));
		}

		private static JObject MapNamespace(Type item, JObject j) {
			var ns = item.FullName.Split('.');
			foreach (var n in ns) {
				if (j.Properties().Any(p => p.Name == n)) {
					j = j[n] as JObject;
				} else {
					j = (j[n] = new JObject()) as JObject;
				}
			}
			return j;
		}

		private class TypeCache {
			public Type Declare { get; set; }
			public Type Implement { get; set; }
			public bool Ever { get; set; }//TODO:reload maybe

			public TypeCache(Type declaration) {
				Declare = declaration;
				if (!declaration.IsAbstract) {
					Implement = declaration;
				}
			}
			public Type GetImplement() {
				if (Ever) {
					return Implement;
				}
				Ever = true;
				return Implement ?? (Implement = Gods.Him.FindImplements(Declare, his.Implements).FirstOrDefault(e => e.GetInterfaces().All(ee => !ee.IsGenericType || ee.GetGenericTypeDefinition() != validatorType))) ?? Declare;
			}
		}
	}
}
