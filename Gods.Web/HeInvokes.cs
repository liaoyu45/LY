using System;
using System.Linq;
using System.Web;

namespace Gods.Web {
	public static partial class Him {
		public static ICacheManager CacheManager { get; set; } = new CacheManager();
		public static IMapper Mapper;
		internal static object Invoke(int typeHash, int methodSign) {
			var type = cache.FirstOrDefault(e => e.Declare.GetHashCode() == typeHash)?.GetImplement();
			if (type == null) {
				throw new DllNotFoundException();
			}
			var method = Gods.Him.GetSignedMethod(type, methodSign) ?? Gods.Him.GetSignedMethod(type, -methodSign);
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
			var ins = type.IsAbstract ? null : Activator.CreateInstance(type);
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
			var ps = method.GetParameters().Select(p => MapObject(p.ParameterType, HttpContext.Current.Request[p.Name])).ToArray();
			var newPs = Validate(ins, method, ps);
			if (Gods.Him.SameQueue(newPs as object[], method.GetParameters().Select(e => e.ParameterType))) {
				ps = newPs as object[];
			} else if (newPs != ps) {
				return newPs;
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
