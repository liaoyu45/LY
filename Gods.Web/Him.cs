using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace Gods.Web {
	public class Him {
		internal static readonly string AjaxKey = nameof(Him) + nameof(Him).GetHashCode();//Him1344150689

		public static string Modules = "bin";
		public static string Implements = "bin";

		private static readonly JObject CSharp = new JObject();
		private static string WebRoot;

		public static void PreferJavascript() {
			PreferJavascript("window");
		}
		public static void PreferJavascript(string thisArg) {
			File.WriteAllText(WebRoot + nameof(Him) + ".js",
				Properties.Resources.Map
				.Replace(nameof(Gods), thisArg)
				.Replace(nameof(CSharp), CSharp.ToString()));
		}

		public static void Create<T>() {
			Validator.TagInterface = typeof(T);
			Validator.WebApp = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly;
			WebRoot = HostingEnvironment.MapPath("/");
			Directory.GetFiles(WebRoot + Modules, "*.dll")
				.Select(e => Gods.Him.TryGet(() => Assembly.LoadFrom(e))).ToList()
				.ForEach(a => {
					var c = a?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
					if (c.Length == 0 || c.StartsWith(nameof(Microsoft)) || c.StartsWith(nameof(Gods)) || c.StartsWith(nameof(Newtonsoft))) {
						return;
					}
					foreach (var t in a.ExportedTypes) {
						if (t.IsInterface && t.GetInterfaces().Any(i => i == typeof(T))) {
							Append(CSharp, t);
							interfaces.Add(t);
						}
					}
				});
		}

		public static void Append(JObject j, Type item) {
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
			item.GetMethods().Where(m => !m.IsSpecialName).ToList().ForEach(m => {
				var t = new JObject {
					[nameof(m.Name)] = m.Name,
					["Key"] = item.GetHashCode() + "." + Gods.Him.SignMethod(m, nameof(Gods))
				};
				var Parameters = JArray.FromObject(m.GetParameters().Select(e => new {
					e.Name, Type = e.ParameterType.FullName
				}));
				if (Parameters.Count > 0) {
					t[nameof(Parameters)] = Parameters;
				}
				var rt = m.ReturnType;
				if (rt == typeof(string) || rt.IsValueType || rt == typeof(void)) {
				} else {
					if (rt.IsGenericType && rt.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
						rt = rt.GenericTypeArguments[0];
					}
					t[nameof(Queryable)] = true;
					t[nameof(MethodInfo.ReturnType)] = GetProperties(rt);
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

		class F<T> {
			[Import]
			public T Ti { get; set; }
			public F(string path) {
				var catalog = new DirectoryCatalog(path);
				var container = new CompositionContainer(catalog);
				container.ComposeParts(this);
			}
		}
		private static List<Type> interfaces = new List<Type>();
		private static Dictionary<string, Type> implements = new Dictionary<string, Type>();

		public static Type TagInterface { get; private set; }

		public static Type FindImplement(string k) {
			if (implements.ContainsKey(k)) {
				return implements[k];
			}
			var tt = interfaces.FirstOrDefault(t => t.GetHashCode().ToString() == k);
			interfaces.Remove(tt);
			var ft = typeof(F<>).MakeGenericType(tt);
			var iti = ft.GetProperty(nameof(F<object>.Ti)).GetValue(Activator.CreateInstance(ft, WebRoot + Implements));
			return implements[k] = iti.GetType();
		}
	}
}
