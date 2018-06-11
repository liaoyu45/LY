using System;
using System.Linq;

namespace Gods.Web {
	public static partial class Him {
		public static ICacheManager CacheManager = new CacheManager();
		internal static object Invoke(int typeHash, int methodSign, Func<string, object> valueMap) {
			var type = cache.FirstOrDefault(e => e.Declare.GetHashCode() == typeHash).GetImplement();
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
			type.GetProperties().Where(p => p.CanWrite).ToList().ForEach(p => {
				var v = MapObject(p.PropertyType, valueMap(p.Name));
				if (v != null) {
					p.SetValue(ins, v);
				}
			});
			var ps = method.GetParameters().Select(p => MapObject(p.ParameterType, valueMap(p.Name))).ToArray(); ;
			var newPs = Validate(ins, method, ps);
			if (newPs != null && ps.Any()) {
				var e = (newPs as object[] ?? new object[0]).Select(p => p?.GetType());
				var ee = ps.Select(p => p.GetType());
				if (e.Except(ee).Any() || ee.Except(e).Any()) {
					(ins as IDisposable)?.Dispose();
					return newPs;
				}
				ps = newPs as object[];
			}
			r = method.Invoke(ins, ps);
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
