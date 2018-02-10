using System;
using System.Linq;

namespace Javascript.Extensions {
	public static class StringExtensions {
		public static bool In(this string self, object target) {
			return true == target?.GetType().GetProperties().Any(e => e.Name == self);
		}
		public static bool In<T>(this string self, object target) {
			return target?.GetType().GetProperties().FirstOrDefault(e => e.Name == self)?.PropertyType == typeof(T);
		}
		public static T ToArray<T>(this string self, params string[] spliter) where T : struct {
			throw new NotImplementedException();
			//self?.Split(spliter, StringSplitOptions.RemoveEmptyEntries).Select;
		}
	}
}
