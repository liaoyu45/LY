using System;
using System.Linq;
using System.Reflection;

namespace Gods.Web {
	public partial class Him {
		private static object[] MapParameters(MethodInfo method, Func<string, object> value) {
			return method.GetParameters().Select(p => MapObject(p.ParameterType, value(p.Name))).ToArray();
		}

		private static object MapObject(Type type, Func<string, object> value) {
			var ins = type.IsAbstract ? null : Activator.CreateInstance(type);
			type.GetProperties().Where(p => p.CanWrite).ToList().ForEach(p => {
				var v = MapObject(p.PropertyType, value(p.Name));
				if (v != null) {
					p.SetValue(ins, v);
				}
			});
			return ins;
		}

		private static object MapObject(Type type, object value) {
			if (value == null) {
				return null;
			}
			if (type == typeof(string)) {
				return value + string.Empty;
			}
			if (type.IsValueType) {
				var pv = value?.ToString().Trim() ?? string.Empty;
				if (pv.Length == 0) {
					return null;
				}
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
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
			return value;
		}
	}
}
