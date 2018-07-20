using System;
using System.Linq;

namespace Gods.Web {
	public partial class Him {
		private static object MapObject(Type type, object value) {
			if (type == typeof(string)) {
				return value?.ToString().Trim();
			}
			if (type.IsValueType) {
				var pv = value?.ToString().Trim() ?? string.Empty;
				if (pv.Length == 0) {
					return Activator.CreateInstance(type);
				}
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					if (pv.Length == 0 || pv.ToLower() == "null") {
						return null;
					}
					type = type.GenericTypeArguments[0];
				}
				if (type == typeof(bool)) {
					return pv.ToLower() != "false" && pv.Any(e => e != '0');
				}
				if (type.IsEnum) {
					int i;
					if (int.TryParse(pv, out i)) {
						return i;
					}
					if (Enum.GetNames(type).Contains(pv)) {
						return Enum.Parse(type, pv);
					}
				}
				if (type.Assembly == typeof(object).Assembly) {
					if (type == typeof(DateTime)) {
						var i = 0L;
						if (long.TryParse(pv, out i)) {
							return new DateTime(i);
						}
					}
					var ops = new object[] { pv, null };
					var ts = new[] { typeof(string), type.MakeByRefType() };
					if ((bool)(type.GetMethod("TryParse", ts)?.Invoke(null, ops) ?? false)) {
						return ops[1];
					}
				}
			}
			return Mapper?.MapObject(type, value) ?? Activator.CreateInstance(type);
		}
	}
}
