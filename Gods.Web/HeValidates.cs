using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	public partial class Him {
		public static object Validate(object obj, MethodInfo calling) {
			if (validatorType == null) {
				return null;
			}
			var s = Gods.Him.SignMethod(calling);
			if (validators.ContainsKey(s)) {
				return validators[s]?.Invoke(obj);
			}
			var validatingType = obj.GetType().GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Contains(tagInterface)) ?? obj.GetType();
			var type = Gods.Him.FindImplements(validatorType.MakeGenericType(validatingType), his.Validators).FirstOrDefault();
			if (type == null) {
				return validators[s] = null;
			}
			var types = new[] { validatingType, typeof(MethodInfo), typeof(HttpContext) };
			var construcor = (from c in type.GetConstructors()
							  let ps = c.GetParameters().Select(ee => ee.ParameterType)
							  where !ps.Except(types).Any()
							  orderby ps.Count() descending
							  select c).FirstOrDefault();
			if (construcor == null) {
				throw new NotSupportedException($"Only {types.Aggregate(string.Empty, (str, k) => str + k + ',')} are supported in limiter's constructor. Invalid limiter' type: {type}.");
			}
			var method = type.GetMethods().FirstOrDefault(e => e.Name == calling.Name);
			var vps = type.GetProperties();
			return (validators[s] = o => {
				var ins = construcor.Invoke(construcor.GetParameters().Select(e => e.ParameterType == validatingType ? o : e.ParameterType == typeof(MethodInfo) ? (object)method : HttpContext.Current).ToArray());
				var hasSession = method != null && HttpContext.Current.Session != null;
				if (hasSession) {
					foreach (var item in vps.Where(e => e.CanWrite)) {
						item.SetValue(ins, HttpContext.Current.Session[item.Name]);
					}
				}
				var r = method?.Invoke(ins, method.GetParameters().Select(p => MapObject(p.ParameterType, HttpContext.Current.Request[p.Name])).ToArray());
				if (hasSession) {
					foreach (var item in vps.Where(e => e.CanRead)) {
						HttpContext.Current.Session[item.Name] = item.GetValue(ins);
					}
				}
				return r;
			})(obj);
		}
	}
}
