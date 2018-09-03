using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	public static partial class Him {
		public static ICacheManager CacheManager { get; set; } = new CacheManager();
		static MethodInfo m = typeof(JsonConvert).GetMethods().First(e => e.IsGenericMethod && e.Name == nameof(JsonConvert.DeserializeObject));
		static object DeserializeObjectInStream(Type type, string json) {
			try {
				return m.MakeGenericMethod(type).Invoke(null, new[] { json });
			} catch {
			}
			return null;
		}

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
			ValidateAttributes(method);
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
			} else {
				if (newPs != null) {
					return newPs;
				}
				ps = MapParameters(method);
			}
			return MapSession(ins, () => method.Invoke(ins, ps));
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
			foreach (var item in pis.Where(IsNormalType)) {
				var attrs = item.GetCustomAttributes<ValidationAttribute>(true);
				foreach (var at in attrs) {
					if (!at.IsValid(HttpContext.Current.Request[item.Name])) {
						throw new TargetInvocationException(method.DeclaringType.FullName + '.' + method.Name, new ValidationException(at.ErrorMessage ?? "参数验证失败"));
					}
				}
			}
		}

		private static object[] MapParameters(MethodInfo method) {
			var plist = new List<object>();
			var pis = method.GetParameters();
			plist.AddRange(pis.Where(IsNormalType).Select(e => MapNormalType(e.ParameterType, HttpContext.Current.Request[e.Name])));
			var p0 = pis.Where(IsNotNormalType).FirstOrDefault()?.ParameterType;
			if (p0 != null) {
				var s = new StreamReader(HttpContext.Current.Request.InputStream, System.Text.Encoding.UTF8);
				var p0Arg = DeserializeObjectInStream(p0, s.ReadToEnd());
				plist.Add(p0Arg);
				s.Dispose();
			}
			return plist.ToArray();
		}
	}
}