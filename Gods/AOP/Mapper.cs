using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
	public class Mapper {
		internal static Dictionary<Guid, Lazy<object>> Mappers = new Dictionary<Guid, Lazy<object>> {
			{ typeof(Model).GUID, new Lazy<object>(() => new Mapper()) }
		};
		public static void SetMapper<M, T>() where M : Model where T : Mapper, new() {
			Mappers.Add(typeof(M).GUID, new Lazy<object>(() => new T()));
		}
		public static object Invoke(Model obj, MethodInfo m) {
			obj.GetType().GetProperties().Where(p => p.CanWrite).ToList().ForEach(p => {
				var v = obj.Mapper.MapObject(p.PropertyType, obj.GetValue(p.Name));
				if (v != null) {
					p.SetValue(obj, v);
				}
			});
			return m?.Invoke(obj, m.GetParameters().Select(p => obj.Mapper.MapObject(p.ParameterType, obj.GetValue(p.Name))).ToArray());
		}
		public virtual object MapObject(Type type, object value) {
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
