using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AllOfMe {
	public class Class1 {
		private static void Append(JObject j, Type item, Func<MethodInfo, bool> predicate) {
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
			item.GetInterfaces().SelectMany(t => t.GetMethods()).Where(predicate).ToList().ForEach(m => {
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
				var rt = m.ReturnType;
				if (rt == typeof(string) || rt.IsValueType || rt == typeof(void)) {
				} else {
					var it = rt.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GenericTypeArguments[0];
					if (it != null) {
						t[nameof(Type.IsArray)] = true;
						rt = it;
					}
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
	}
}