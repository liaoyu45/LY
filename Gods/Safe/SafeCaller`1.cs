using System;
using System.Linq;
using System.Linq.Expressions;

namespace Gods {
	public class SafeCaller<T> : SafeCaller {
		public SafeCaller(bool debug = true) : base(typeof(T), debug) { }

		public TR Call<TR>(Expression<Func<T, TR>> expression) {
			var m = expression.Body as MethodCallExpression;
			var args = m.Arguments.Select(e => {
				if (e is ConstantExpression cs) {
					return cs.Value;
				}
				var v = e.GetType().GetProperty("Expression").GetValue(e);
				var vvv = v.GetType().GetProperty("Value").GetValue(v);
				return vvv.GetType().GetFields()[0].GetValue(vvv);
			}).ToArray();
			return (TR)base.Call(m.Method.Name, args);
		}
	}
}
