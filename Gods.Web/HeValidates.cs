using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	public partial class Him {
		public static object Validate(object obj, MethodInfo calling, params object[] parameters) {
			if (validatorType == null) {
				return null;
			}
			var s = Gods.Him.SignMethod(calling);
			if (validators.ContainsKey(s)) {
				return validators[s](obj, calling, parameters);
			}
			var ot = obj.GetType();
			var vi = ot.GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Contains(tagInterface)) ?? ot;
			var vt = Gods.Him.FindImplements(validatorType.MakeGenericType(vi), his.Validators).FirstOrDefault();
			var method = Gods.Him.GetMappedMethod(vt, calling) ?? vt.GetMethods().FirstOrDefault(e => e.Name == calling.Name);
			return (validators[s] = (o, m, ps) => {
				if (method == null) {
					return ps;
				}
				var cps = vt.GetConstructors().OrderBy(e => e.GetParameters().Length).LastOrDefault().GetParameters().Select(e => e.ParameterType);
				if (cps.Except(new[] { vi, typeof(MethodInfo), typeof(HttpContext) }).Any()) {
					throw new NotSupportedException($"Only {ot} and {typeof(MethodInfo)} are supported in limiter's constructor. Invalid limiter' type: {vt}.");
				}
				var r = method.Invoke(Activator.CreateInstance(vt, cps.Select(e => e == vi ? o : e == typeof(MethodInfo) ? (object)m : HttpContext.Current).ToArray()), method.GetParameters().Length > 0 ? ps : null);
				return method.ReturnType == typeof(void) ? ps : r;
			})(obj, calling, parameters);
		}
	}
}
