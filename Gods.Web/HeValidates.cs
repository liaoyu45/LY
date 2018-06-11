using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.Web {
	public partial class Him {
		public static string Implements = "bin";
		public static string Validators = "bin";
		private static Type validatorType;

		private static Dictionary<int, Func<object, object[], object>> validators = new Dictionary<int, Func<object, object[], object>>();

		public static object Validate(object obj, MethodInfo m, params object[] ps) {
			if (validatorType == null) {
				return null;
			}
			var c = Gods.Him.SignMethod(m);
			if (validators.ContainsKey(c)) {
				return validators[c](obj, ps);
			}
			var fi = obj.GetType().GetInterfaces().First(i => i.GetInterfaces().Any(ii => ii == TagInterface));
			var vt = Gods.Him.FindInstance(validatorType.MakeGenericType(fi), webRoot + Validators)?.GetType();
			var mapped = Gods.Him.GetMappedMethod(vt, m) ?? vt?.GetMethods().FirstOrDefault(mm => mm.Name == m.Name && mm.GetParameters().Length == 0);
			if (mapped == null) {
				return ps;
			}
			return (validators[c] = (tar, pss) => {
				var ins = Activator.CreateInstance(vt);
				vt.GetFields((BindingFlags)36).FirstOrDefault(f => f.DeclaringType == fi)?.SetValue(ins, tar);
				vt.GetProperties((BindingFlags)36).FirstOrDefault(f => f.DeclaringType == fi && f.CanWrite)?.SetValue(ins, tar);
				return mapped.Invoke(ins, mapped.GetParameters().Any() ? pss : null);
			})(obj, ps);
		}
	}
}
