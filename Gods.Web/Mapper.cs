using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	public partial class Him {
		private static readonly Type[] NormalTypes = { typeof(string), typeof(byte[]), typeof(Stream) };
		public static bool IsNormalType(Type type) {
			return type.IsValueType || NormalTypes.Contains(type);
		}

		public static object MapNormalType(Type type, string name) {
			var value = HttpContext.Current.Request[name];
			if (type == typeof(string)) {
				return value?.Trim();
			}
			if (type == typeof(byte[])) {
				var s = HttpContext.Current.Request.Files[name]?.InputStream;
				var bs = new byte[s?.Length ?? 0];
				s?.Read(bs, 0, bs.Length);
				return bs;
			}
			if (type == typeof(Stream)) {
				return HttpContext.Current.Request.InputStream;
			}
			var pv = value?.Trim() ?? string.Empty;
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
			return null;
		}
	}
}
