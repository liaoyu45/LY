using System;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gods.Web {
	public partial class Him {
		public static object Validate(object obj, MethodInfo calling) {
			if (validatorType == null) {
				return null;
			}
			var s = SignMethod(calling);
			if (validators.ContainsKey(s)) {
				return validators[s]?.Invoke(obj);
			}
			var validatingType = obj.GetType().GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Contains(tagInterface)) ?? obj.GetType();
			var type = FindImplements(validatorType.MakeGenericType(validatingType), his.Validators).FirstOrDefault();
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
				var fs = type.GetFields((BindingFlags)36);
				foreach (var item in fs) {
					if (item.FieldType == typeof(HttpContext)) {
						item.SetValue(ins, HttpContext.Current);
					}
					if (item.FieldType == validatingType) {
						item.SetValue(ins, o);
					}
				}
				return MapSession(ins, () =>
					method?.Invoke(ins, method.GetParameters().Select(p => MapNormalType(p.ParameterType, p.Name)).ToArray())
				);
			})(obj);
		}
	}
}
