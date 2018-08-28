using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	public static partial class Him {
		public static ICacheManager CacheManager { get; set; } = new CacheManager();
		public static IMapper Mapper;
		internal static object Invoke(int typeNameHash, int methodSign) {
			var type = cache.FirstOrDefault(e => e.GetHashCode() == typeNameHash)?.GetImplement();
			return Invoke(type, Gods.Him.GetSignedMethod(type, methodSign) ?? Gods.Him.GetSignedMethod(type, -methodSign));
		}

		internal static object Invoke(string typeName, string methodName) {
			var type = cache.FirstOrDefault(e => e.GetHashCode() == typeName.GetHashCode()).GetImplement();
			return Invoke(type, type.GetMethods().FirstOrDefault(e => e.Name == methodName));
		}

		private static object Invoke(Type type, MethodInfo method) {
			if (method == null) {
				throw new MissingMethodException();
			}
			if (type.IsInterface) {
				return Activator.CreateInstance(method.ReturnType);
			}
			object r;
			var a = CacheManager?.Cacheable(method) ?? 0;
			if (a > 0) {
				r = CacheManager.Read(a);
				if (r != null) {
					return r;
				}
			}
			var bad = method.GetParameters().FirstOrDefault(e => e.GetCustomAttributes<RequiredAttribute>().Any() && HttpContext.Current.Request.Params.AllKeys.Contains(e.Name));
			if (bad != null) {
				throw new TargetParameterCountException(method.Name + " needs parameter: " + bad.Name);
			}
			var ins = Activator.CreateInstance(type);
			var prs = type.GetProperties();
			var hasSession = HttpContext.Current.Session != null;
			if (hasSession) {
				foreach (var item in prs.Where(e => e.CanWrite)) {
					var v = HttpContext.Current.Session[item.Name];
					if (v != null) {
						item.SetValue(ins, v);
					}
				}
			}
			var newPs = Validate(ins, method);
			object[] ps;
			if (newPs != null) {
				if (newPs is object[]) {
					var nps = (newPs as object[]).Select(e => e?.GetType()).ToArray();
					var ops = method.GetParameters().Select(e => e.ParameterType).ToArray();
					if (nps.Length != ops.Length) {
						return newPs;
					}
					for (int i = 0; i < ops.Length; i++) {
						if (nps[i] != null && nps[i] != ops[i]) {
							return newPs;
						}
					}
					ps = newPs as object[];
				} else {
					return newPs;
				}
			} else {
				ps = method.GetParameters().Select(p => MapObject(p.ParameterType, HttpContext.Current.Request[p.Name])).ToArray();
			}
			r = method.Invoke(ins, ps);
			if (hasSession) {
				foreach (var item in prs.Where(e => e.CanRead)) {
					HttpContext.Current.Session[item.Name] = item.GetValue(ins);
				}
			}
			if (a > 0) {
				CacheManager.Save(a, r);
			} else {
				CacheManager?.Remove(a);
			}
					(ins as IDisposable)?.Dispose();
			return r;
		}
	}
}