using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.Web {
	public partial class Him {
		public static object Validate(object obj, MethodInfo m, params object[] ps) {
			if (validatorType == null) {
				return null;
			}
			var s = Gods.Him.SignMethod(m);
			if (validators.ContainsKey(s)) {
				return validators[s](obj, ps);
			}
			var fi = obj.GetType().GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Any(ii => ii == tagInterface)) ?? obj.GetType();
			var vt = Gods.Him.FindInstance(validatorType.MakeGenericType(fi), his.Validators)?.GetType();
			if (vt == null) {
				return null;
			}
			var mapped = Gods.Him.GetMappedMethod(vt, m) ?? vt.GetMethods().FirstOrDefault(mm => mm.Name == m.Name && mm.GetParameters().Length == 0);
			if (mapped == null) {//TODO:continue here
				var c = vt.GetConstructors().OrderByDescending(e => e.GetParameters().Length).FirstOrDefault();
				var p = c?.GetParameters().Select(e => e.ParameterType);
				if (c == null || p.Except(new[] { typeof(object[]), obj.GetType(), typeof(MethodInfo) }).Any()) {
					throw new NotSupportedException("");
				}
				var ars = new List<object>();
				if (p.Contains(typeof(object[]))) {
					ars.Add(ps);
				}
				if (p.Contains(typeof(MethodInfo))) {
					ars.Add(m);
				}
				if (p.Contains(obj.GetType())) {
					ars.Add(obj);
				}
				Activator.CreateInstance(vt, ars.ToArray());
				return ps;
			}
			return (validators[s] = (objj, pss) => {
				var ins = Activator.CreateInstance(vt);
				vt.GetFields((BindingFlags)36).FirstOrDefault(f => f.DeclaringType == fi)?.SetValue(ins, objj);
				vt.GetProperties((BindingFlags)36).FirstOrDefault(f => f.DeclaringType == fi && f.CanWrite)?.SetValue(ins, objj);
				return mapped.Invoke(ins, mapped.GetParameters().Any() ? pss : null);
			})(obj, ps);
		}
	}
}
