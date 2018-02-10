using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.Web {
	class Validator : IValidator {
		internal static Assembly WebApp;
		internal static Validator Instance = new Validator();

		private static List<int> wontValidate = new List<int>();
		private static Dictionary<int, Func<object, object[], object>> validators = new Dictionary<int, Func<object, object[], object>>();

		public object Validate(object obj, MethodInfo m, params object[] ps) {
			var c = Gods.Him.SignMethod(m, m.DeclaringType);
			if (wontValidate.Contains(c)) {
				return null;
			}
			if (validators.ContainsKey(c)) {
				return validators[c](obj, ps);
			}
			var it = obj.GetType().GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Any(ii => ii == Him.TagInterface));
			if (it == null) {
				wontValidate.Add(c);
				return null;
			}
			var vt = typeof(Validator<>).MakeGenericType(it);
			vt = WebApp.DefinedTypes.FirstOrDefault(t => t.IsSubclassOf(vt));
			if (vt == null) {
				wontValidate.Add(c);
				return null;
			}
			var mm = Gods.Him.GetMappedMethod(vt, m, null);
			if (mm == null) {
				wontValidate.Add(c);
				return null;
			}
			var ins = Activator.CreateInstance(vt);
			validators[c] = (objs, pss) => {
				vt.GetProperty(nameof(Validator<object>.Target)).SetValue(ins, objs);
				return mm.Invoke(ins, pss);
			};
			return validators[c](obj, ps);
		}
	}
	public class Validator<T> {
		public T Target { get; set; }
	}

	public interface IValidator {
		object Validate(object obj, MethodInfo method, params object[] ps);
	}
}
