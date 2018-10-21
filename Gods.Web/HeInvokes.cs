using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Gods.Web {
	public partial class Him {
		public static ICacheManager CacheManager { get; set; }
		static MethodInfo m = typeof(JsonConvert).GetMethods().First(e => e.IsGenericMethod && e.Name == nameof(JsonConvert.DeserializeObject));

		internal static object Invoke(int typeNameHash, int methodSign) {
			var type = AllTypeCache.FirstOrDefault(e => e.GetHashCode() == typeNameHash)?.GetImplement();
			return Invoke(type, GetSignedMethod(type, methodSign) ?? GetSignedMethod(type, -methodSign));
		}

		internal static object Invoke(string typeName, string methodName) {
			var c = AllTypeCache.FirstOrDefault(e => e.GetHashCode() == typeName.GetHashCode());
			return Invoke(c?.GetImplement(), c?.Declare.GetMethods().FirstOrDefault(e => e.Name == methodName));
		}

		private static object Invoke(Type type, MethodInfo method) {
			if (method == null) {
				throw new MissingMethodException();
			}
			if (type.IsInterface) {
				return method == typeof(void) ? null : Activator.CreateInstance(method.ReturnType);
			}
			var k = CacheManager?.Cacheable(method) ?? 0;
			if (k > 0) {
				var r = CacheManager.Read(k);
				if (r == null) {
					r = RealInvoke(Activator.CreateInstance(type), method);
					CacheManager.Save(k, r);
					return r;
				}
				return r;
			}
			return RealInvoke(Activator.CreateInstance(type), method);
		}

		private static object RealInvoke(object ins, MethodInfo method) {
			object[] ps = null;
			var newPs = Validate(ins, method);
			if (newPs is object[]) {
				ps = newPs as object[];
				if (WillIntercept(method, ps)) {
					return newPs;
				}
			} else if (newPs is Dictionary<string, object>) {
				var d = newPs as Dictionary<string, object>;
				if (WillIntercept(method, d)) {
					return newPs;
				}
				var od = MapParameters(method);
				foreach (var item in d.Keys) {
					od[item] = d[item];
				}
				ps = od.Values.ToArray();
			} else if (newPs != null) {
				return newPs;
			} else {
				ps = ps ?? MapParameters(method).Values.ToArray();
			}
			return MapSession(ins, () => method.Invoke(ins, ps));
		}

		private static bool WillIntercept(MethodInfo method, Dictionary<string, object> dictionary) {
			var ps = method.GetParameters();
			return dictionary.Keys.Except(ps.Select(a => a.Name)).Any() || dictionary.Values.Where(a => a != null).Select(a => a.GetType()).Except(ps.Select(a => a.ParameterType)).Any();
		}

		private static bool WillIntercept(MethodInfo method, object[] newPs) {
			var nps = newPs.Select(e => e?.GetType()).ToArray();
			var pis = method.GetParameters();
			var ops = pis.Select(e => e.ParameterType).ToArray();
			if (nps.Length != ops.Length) {
				return true;
			}
			for (int i = 0; i < ops.Length; i++) {
				if (nps[i] != null && nps[i] != ops[i]) {
					return true;
				}
			}
			return false;
		}

		private static void ValidateAttributes(MethodInfo method) {
			var pis = method.GetParameters();
			foreach (var item in pis) {
				var attrs = item.GetCustomAttributes<ValidationAttribute>(true);
				foreach (var at in attrs) {
					if (!at.IsValid(HttpContext.Current.Request[item.Name])) {
						throw new TargetInvocationException(method.DeclaringType.FullName + '.' + method.Name, new ValidationException(at.ErrorMessage ?? "参数验证失败"));
					}
				}
			}
		}

		private static Dictionary<string, object> MapParameters(MethodInfo method) {
			ValidateAttributes(method);
			var para = method.GetParameters().FirstOrDefault(a => !IsNormalType(a));
			if (para != null) {
				object v;
				var req = HttpContext.Current.Request;
				var ps = para.ParameterType.GetProperties().Where(e => e.CanWrite);
				if (req.Form.Cast<string>().Concat(req.QueryString.Cast<string>()).Intersect(ps.Select(a => a.Name)).Any()) {
					v = Activator.CreateInstance(para.ParameterType);
					foreach (var item in ps) {
						item.SetValue(v, MapNormalType(item.PropertyType, item.Name));
					}
					return new Dictionary<string, object> { { para.Name, v } };
				} else {
					var bs = new byte[HttpContext.Current.Request.InputStream.Length];
					HttpContext.Current.Request.InputStream.Read(bs, 0, bs.Length);
					v = m.MakeGenericMethod(para.ParameterType).Invoke(null, new[] { Encoding.UTF8.GetString(bs) });
					return new Dictionary<string, object> { { para.Name, v } };
				}
			}
			return method.GetParameters().ToDictionary(e => e.Name, e => MapNormalType(e.ParameterType, e.Name));
		}
	}
}