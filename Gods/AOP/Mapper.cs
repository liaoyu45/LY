using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
	public class Mapper : IMapper {
		internal static Dictionary<Guid, Lazy<object>> Mappers = new Dictionary<Guid, Lazy<object>> {
			{ typeof(Model).GUID, new Lazy<object>(() => new Mapper()) }
		};
		public static void SetMapper<M, T>() where M : Model where T : Mapper, new() {
			Mappers.Add(typeof(M).GUID, new Lazy<object>(() => new T()));
		}
		public static object Invoke(Model obj, MethodInfo m) {
			MapObject(obj, obj.Mapper, obj.GetValue);
			return m?.Invoke(obj, MapParameters(m, obj.Mapper, obj.GetValue));
		}
		public static object[] MapParameters(MethodInfo method, IMapper mapper, Func<string, object> value) {
			return method.GetParameters().Select(p => mapper.MapObject(p.ParameterType, value(p.Name))).ToArray();
		}
		public static object[] MapParameters(MethodInfo method, Func<string, object> value) {
			return MapParameters(method, new Mapper(), value);
		}
		public static void MapObject(object obj, IMapper m, Func<string, object> value) {
			obj.GetType().GetProperties().Where(p => p.CanWrite).ToList().ForEach(p => {
				var v = m.MapObject(p.PropertyType, value(p.Name));
				if (v != null) {
					p.SetValue(obj, v);
				}
			});
		}
		public static void MapObject(object obj, Func<string, object> value) {
			MapObject(obj, new Mapper(), value);
		}
		public static readonly IMapper Instance = new Mapper();

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
