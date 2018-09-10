using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private static Dictionary<int, Func<object, object>> validators = new Dictionary<int, Func<object, object>>();

		public static List<TypeCache> AllTypeCache = new List<TypeCache>();

		public static His his;

		public static void Create<T>() {
			Create<T>(new His());
		}
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
			var webRoot = HostingEnvironment.MapPath("/");
			var root = $"{webRoot}/Scripts";
			his.Implements = webRoot + his.Implements;
			his.Validators = webRoot + his.Validators;
			his.Modules = webRoot + his.Modules;
			var fs = FindImplements(tagInterface, his.Modules).Where(e => e.IsInterface);
			if (fs.GroupBy(e => e.Assembly).Select(e => e.Key).Any(e => e.ExportedTypes.Any(ee => ee.IsClass && ee.GetInterfaces().Contains(tagInterface)))) {
				throw new BadImageFormatException("All classes in module assembly can not inherit the tag interface.");
			}
			Him.his = his;
			RouteTable.Routes.Add(new Route(his.AjaxRoute, new Me()));
			fs.ToList().ForEach(Append);
			RouteTable.Routes.Add(new Route(nameof(CSharp), new JsWriter(CSharp)));
			RouteTable.Routes.Add(new Route(nameof(Javascript), new JsWriter(Javascript)));
		}

		private static void Append(Type item) {
			var javascript = MapNamespace(item, Javascript)[item.Name] = new JObject();
			var csharp = MapNamespace(item, CSharp);
			var Methods = new JArray();
			item.GetMethods().Where(m => !m.IsSpecialName).ToList().ForEach(m => {
				javascript[m.Name] = m.GetCustomAttribute<DescriptionAttribute>()?.Description;
				var t = new JObject {
					[nameof(m.Name)] = m.Name,
					["Key"] = item.FullName.GetHashCode() + "." + Math.Abs(SignMethod(m))
				};
				Methods.Add(t);
				var Parameters = JArray.FromObject(m.GetParameters().Select(e => e.Name).ToList());
				t[nameof(Parameters)] = Parameters;
				var rt = ToFlatObject(m.ReturnType);
				if (rt != null) {
					t["Return"] = rt;
				}
			});
			if (Methods.Count > 0) {
				csharp[item.Name] = Methods;
				var v = $"{his.AjaxRoute}/{Methods.Path.Replace('.', '/')}/";
				foreach (var m in Methods) {
					RouteTable.Routes.Add(new Route(v + m["Name"], new Me()));
				}
			}
			AllTypeCache.Add(new TypeCache(item));
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
	}
}
