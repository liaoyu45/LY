using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	static class Mapper {
		internal static object MapProperties(object r) {
			foreach (var item in r.GetType().GetProperties()) {
				if (item.CanWrite) {
					var v = Map(item.PropertyType, item.Name);
					if (item.PropertyType.Equals(v?.GetType())) {
						item.SetValue(r, v);
					}
				}
			}
			return r;
		}
		internal static object[] MapParameters(MethodInfo m) {
			return m.GetParameters().Select(p => Map(p.ParameterType, p.Name)).ToArray();
		}
		private static object Map(Type pt, string name) {
			var req = HttpContext.Current.Request;
			var fs = HttpContext.Current.Request.Files;
			if (pt == typeof(string)) {
				return req[name];
			} else if (pt == typeof(byte[])) {
				if (fs.AllKeys.Contains(name)) {
					var s = fs[name].InputStream;
					var bs = new byte[s.Length];
					s.Read(bs, 0, bs.Length);
					return bs;
				} else {
					try {
						return Convert.FromBase64String(req[name]);
					} catch {
					}
				}
			} else if (pt == typeof(MemoryStream)) {
				if (fs.AllKeys.Contains(name)) {
					var m = new MemoryStream();
					fs[name].InputStream.CopyTo(m);
					return m;
				}
			} else if (pt.IsValueType) {
				var pv = req[name]?.Trim() ?? string.Empty;
				if (pv.Length == 0) {
					return null;
				}
				if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					pt = pt.GenericTypeArguments[0];
				}
				if (pt == typeof(bool)) {
					return pv.ToLower() != "false" && pv.Any(e => e != '0');
				} else if (pt.IsEnum) {
					int i;
					if (int.TryParse(pv, out i)) {
						return i;
					}
					if (Enum.GetNames(pt).Contains(pv)) {
						return Enum.Parse(pt, pv);
					}
				} else if (pt.Assembly == typeof(object).Assembly) {
					if (pt == typeof(DateTime)) {
						var i = 0L;
						if (long.TryParse(pv, out i)) {
							return new DateTime(i);
						}
					}
					var ops = new object[] { pv, null };
					var ts = new[] { typeof(string), pt.MakeByRefType() };
					if ((bool)(pt.GetMethod("TryParse", ts)?.Invoke(null, ops) ?? false)) {
						return ops[1];
					}
				}
			}
			return null;
		}
	}
}
