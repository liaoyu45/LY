using System;
using System.Linq;
using System.Reflection;

namespace Javascript.Extensions {
	public static class ObjectExtensions {
		public static void ForIn(this object self, Func<PropertyInfo, object> map) {
			foreach (var p in self.GetType().GetProperties().Where(p => p.CanWrite)) {
				var v = map(p);
				if (v?.GetType() == p.PropertyType) {
					p.SetValue(self, v);
				}
			}
		}

		public static T CopyTo<T>(this object fromA, params string[] excludes) where T : new() =>
			CopyTo<T>(fromA, true, excludes);

		public static T CopyTo<T>(this object fromA, bool contains, params string[] props) where T : new() {
			var toB = new T();
			CopyTo(fromA, toB, contains, props);
			return toB;
		}
		public static void CopyTo(this object fromA, object toB, params string[] excludes) =>
			CopyTo(fromA, toB, true, excludes);
		public static void CopyTo(this object fromA, object toB, bool contains, params string[] props) {
			if (fromA == null || toB == null) {
				return;
			}
			var wontCheck = props.Length == 0;
			toB.GetType().GetProperties()
				.Where(p => p.CanWrite && (wontCheck || props.Contains(p.Name) == contains))
				.ToList().ForEach(p => {
					var va = fromA.GetType().GetProperty(p.Name)?.GetValue(fromA);
					if (va?.GetType() == p.PropertyType) {
						p.SetValue(toB, va);
					}
				});
		}
	}
}
