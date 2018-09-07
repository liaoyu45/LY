using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Gods.Web {
	partial class Him {
		public static JToken ToFlatObject(Type rt) {
			var flat = toFlat(rt);
			if (flat != null) {
				return flat;
			}
			if (rt == typeof(void)) {
				return null;
			}
			var isArr = rt.GetInterfaces().Any(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (isArr) {
				rt = rt.IsArray ? rt.GetElementType() : rt.GenericTypeArguments[0];
			}
			IDictionary<string, object> v = new ExpandoObject();
			foreach (var item in rt.GetProperties()) {
				v.Add(item.Name, toFlat(item.PropertyType));
			}
			return isArr ? (JToken)JArray.FromObject(new[] { v }) : JObject.FromObject(v);
		}

		private static JToken toFlat(Type rt) {
			if (rt == typeof(string)) {
				return string.Empty;
			}
			if (rt == typeof(DateTime)) {
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}
			if (rt == typeof(void)) {
				return null;
			}
			if (rt.IsValueType) {
				return 0;
			}
			return null;
		}
	}
}
