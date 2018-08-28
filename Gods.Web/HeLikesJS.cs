using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Routing;

namespace Gods.Web {
	public partial class Him {
		private static readonly JObject CSharp = new JObject();
		private static readonly JObject Javascript = new JObject();
		private static Type tagInterface;
		private static Type validatorType;
		private static Dictionary<int, Func<object, object>> validators = new Dictionary<int, Func<object, object>>();
		private static List<TypeCache> cache = new List<TypeCache>();
		internal static His his;

		public static void Create<T>(His his) {
			if (!System.Text.RegularExpressions.Regex.IsMatch(his.AjaxRoute, "[a-zA-Z_][a-zA-Z_0-9]*")) {
				throw new ArgumentException("invalid " + nameof(His.AjaxRoute), nameof(his));
			}
			var t = typeof(T);
			if (!t.IsInterface) {
				throw new ArgumentException("T should be an interface.");
			}
			if (t.IsGenericType) {
				if (t.GenericTypeArguments.Length > 1 || !t.GenericTypeArguments[0].IsInterface || t.GenericTypeArguments[0].IsGenericType) {
					throw new ArgumentException("If T is generic, T should have only one type argument 'X', 'X' should be an interface, 'X' can't be generic.");
				}
				validatorType = t.GetGenericTypeDefinition();
				tagInterface = t.GenericTypeArguments[0];
			} else {
				tagInterface = t;
			}
			Him.his = his;
			RouteTable.Routes.Add(new Route(his.AjaxRoute, new Me()));
			var webRoot = HostingEnvironment.MapPath("/");
			var root = $"{webRoot}/Scripts";
			his.Implements = webRoot + his.Implements;
			his.Validators = webRoot + his.Validators;
			his.Modules = webRoot + his.Modules;
			Gods.Him.FindImplements(tagInterface, his.Modules).Where(e => e.IsInterface).ToList().ForEach(Append);
			foreach (var item in CSharp) {
				File.WriteAllText($"{root}/{nameof(CSharp)}/{item.Key}.js", $@"
window.god = window.god || (window.god = {{}});
(god.CSharp || (god.CSharp = {{}})).{item.Key} = {item.Value.ToString(Formatting.Indented, new JavaScriptDateTimeConverter())};".Trim());
			}
			foreach (var item in Javascript) {
				var v = $"{root}/{nameof(Javascript)}/{item.Key}.js";
				if (!File.Exists(v)) {
					File.WriteAllText(v, $@"
/// <reference path=""/Scripts/CSharp/{item.Key}.js"" />
/// <reference path=""/Scripts/god.web.js"" />
god.MakeJavasciptLookLikeCSharp(""{item.Key}"",{item.Value.ToString(Formatting.Indented)});".Trim());
				}
			}
		}

		private static void Append(Type item) {
			var javascript = MapNamespace(item, Javascript)[item.Name] = new JObject();
			var csharp = MapNamespace(item, CSharp);
			var Methods = new JArray();
			item.GetMethods().Where(m => !m.IsSpecialName).ToList().ForEach(m => {
				javascript[m.Name] = null;
				var t = new JObject {
					[nameof(m.Name)] = m.Name,
					["Key"] = item.FullName.GetHashCode() + "." + Math.Abs(Gods.Him.SignMethod(m))
				};
				Methods.Add(t);
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
				} else if (rt == typeof(void)) {
					return;
				} else if (rt.IsValueType) {
					v = 0;
				} else if (rt.GetInterfaces().Any(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
					rt = rt.IsArray ? rt.GetElementType() : rt.GenericTypeArguments[0];
					v = JArray.FromObject(new[] { Activator.CreateInstance(rt) });
				} else {
					v = JToken.FromObject(Activator.CreateInstance(rt));
				}
				t["Return"] = v;
			});
			if (Methods.Count > 0) {
				csharp[item.Name] = Methods;
				var v = $"{his.AjaxRoute}/{Methods.Path.Replace('.', '/')}/";
				foreach (var m in Methods) {
					RouteTable.Routes.Add(new Route(v + m["Name"], new Me()));
				}
			}
			cache.Add(new TypeCache(item));
		}

		private static JObject MapNamespace(Type item, JObject j) {
			var ns = item.FullName.Split('.').Reverse().Skip(1).Reverse();
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
			private Type declare;
			private Type implement;
			private bool ever;//TODO:reload maybe

			public TypeCache(Type declare) {
				this.declare = declare;
				if (!declare.IsAbstract) {
					implement = declare;
				}
			}
			public Type GetImplement() {
				if (ever) {
					return implement;
				}
				ever = true;
				return implement = Gods.Him.FindImplements(declare, his.Implements).FirstOrDefault(e => !e.IsAbstract && e.GetConstructors().Any(ee => ee.GetParameters().Length == 0)) ?? declare;
			}

			public override int GetHashCode() {
				return declare.FullName.GetHashCode();
			}
		}
	}
}
