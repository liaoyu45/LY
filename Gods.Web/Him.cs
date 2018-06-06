using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.Web {
	public partial class Him {
		public static string Implements = "bin";
		private static Assembly ValidatorAssembly;
		private static TypeInfo baseInterface;

		private static Dictionary<int, Func<object, object[], object>> validators = new Dictionary<int, Func<object, object[], object>>();

		public static void SetValidator(Assembly ass) {
			baseInterface = (ValidatorAssembly = ass).DefinedTypes.FirstOrDefault(e => e.IsGenericType && e.IsInterface);
		}

		public static object Validate(object obj, MethodInfo m, params object[] ps) {
			var c = Gods.Him.SignMethod(m);
			if (validators.ContainsKey(c)) {
				return validators[c](obj, ps);
			}
			var it = obj.GetType().GetInterfaces().First(i => i.GetInterfaces().Any(ii => ii == TagInterface));
			var vt = baseInterface.MakeGenericType(it);
			vt = ValidatorAssembly.DefinedTypes.FirstOrDefault(t => !t.IsInterface && t.GetInterfaces().Contains(vt));
			var mapped = Gods.Him.GetMappedMethod(vt, m) ?? vt.GetMethods().FirstOrDefault(mm => mm.Name == m.Name && mm.GetParameters().Length == 0);
			if (mapped == null) {
				return null;
			}
			return (validators[c] = (tar, pss) => {
				var ins = vt.IsAbstract ? null : Activator.CreateInstance(vt);
				vt.GetFields((BindingFlags)36).FirstOrDefault(f => f.DeclaringType == it)?.SetValue(ins, tar);
				vt.GetProperties((BindingFlags)36).FirstOrDefault(f => f.DeclaringType == it && f.CanWrite)?.SetValue(ins, tar);
				return mapped.Invoke(ins, mapped.GetParameters().Any() ? pss : null);
			})(obj, ps);
		}
	}
}
