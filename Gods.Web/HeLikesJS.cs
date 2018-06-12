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
		private static Type tagInterface;
		private static Type validatorType;
		private static Dictionary<int, Func<object, object[], object>> validators = new Dictionary<int, Func<object, object[], object>>();
		private static List<TypeCache> cache = new List<TypeCache>();
		internal static His his;

		public static int HashCode => CSharp.GetHashCode();

		public static string LoadJavascript(string name) {
			return !CSharp.ContainsKey(name) ? null : Properties.Resources.Map1
				.Replace(nameof(His.AjaxRoute), his.AjaxRoute)
				.Replace(nameof(His.AjaxKey), his.AjaxKey)
				.Replace("Value", CSharp.GetValue(name)?.ToString(Newtonsoft.Json.Formatting.Indented))
				.Replace(nameof(GetHashCode), CSharp.GetHashCode().ToString())
				.Replace("Filename", name);
		}

		public static void Create<T>(His his) {
			var t = typeof(T);
			if (!t.IsInterface) {
				throw new ArgumentException("T should be an interface.");
			}
			const string pattern = "[a-zA-Z_][a-zA-Z_0-9]*";
			if (!System.Text.RegularExpressions.Regex.IsMatch(his.AjaxRoute, pattern)) {
				throw new ArgumentException(nameof(His.AjaxRoute) + " should match " + pattern);
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
			his.SetRoot(HostingEnvironment.MapPath("/"));
			Directory.GetFiles(his.Modules, "*.dll").SelectMany(a => {
				try {
					return Assembly.LoadFrom(a)?.ExportedTypes.Where(e => e.IsInterface && !e.IsGenericType && e.GetInterfaces().Contains(tagInterface));
				} catch {
					return Enumerable.Empty<Type>();
				}
			}).ToList().ForEach(Append);
		}

		private static void Append(Type item) {
			var j = CSharp;
			var ns = item.FullName.Split('.');
			foreach (var n in ns) {
				if (j.Properties().Any(p => p.Name == n)) {
					j = j[n] as JObject;
				} else {
					j = (j[n] = new JObject()) as JObject;
				}
			}
			var ps = item.GetProperties().Where(e => e.CanWrite).ToList();
			if (ps.Count > 0) {
				j["Properties"] = JArray.FromObject(ps.Select(e => e.Name));
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
			cache.Add(new TypeCache(item));
		}

		private class TypeCache {
			public Type Declare { get; set; }
			public Type Implement { get; set; }

			public TypeCache(Type declaration) {
				Declare = declaration;
			}
			public Type GetImplement() {
				return Implement ?? (Implement = Gods.Him.FindInstance(Declare, his.Implements)?.GetType()) ?? Declare;
			}
		}
	}
}
