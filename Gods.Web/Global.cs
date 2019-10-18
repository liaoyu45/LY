using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods {
	public static partial class Him {
		private static readonly Random rdm = new Random();

		public static int MinListInstanceCount = -1;
		public static int MaxListInstanceCount = -1;
		/// <summary>
		/// 类似 <see cref="Activator.CreateInstance(ActivationContext)"/>，但属性是随机的。
		/// </summary>
		/// <param name="type">要创建实例的类型。</param>
		/// <returns>具有随机属性的实例。</returns>
		public static object CreateInstance(Type type) {
			var t = new System.Diagnostics.StackTrace();
			if (t.GetFrames().Count(e => e.GetMethod().DeclaringType == typeof(Him)) > 11) {
				return null;
			}
			if (type == null) {
				return null;
			}
			if (type == typeof(string)) {
				return rdm.Next().ToString("x");
			}
			if (type == typeof(int)) {
				return rdm.Next();
			}
			if (type == typeof(bool)) {
				return rdm.NextDouble() < .5;
			}
			if (type == typeof(DateTime)) {
				return DateTime.MinValue.AddTicks((long)(rdm.NextDouble() * DateTime.MaxValue.Ticks));
			}
			if (type.IsEnum) {
				var vs = Enum.GetValues(type).OfType<object>().Select(e => (int)e).ToList();
				if (IsEnum1248(type)) {
					return rdm.Next(1, vs.Max() * 2);
				}
				return vs[rdm.Next(vs.Count)];
			}
			if (type.GetConstructors().Any(e => e.GetParameters().Length == 0)) {
				var r = Activator.CreateInstance(type);
				InitInstance(r);
				return r;
			}
			if (type.IsArray) {
				var min = Math.Max(3, MinListInstanceCount);
				var max = Math.Max(min + 3, MaxListInstanceCount);
				var a = Array.CreateInstance(type.GetElementType(), rdm.Next(min, max));
				for (var i = 0; i < a.Length; i++) {
					a.SetValue(CreateInstance(type.GetElementType()), i);
				}
				return a;
			}
			return null;
		}

		public static bool IsEnum1248(Type t) {
			if (t.IsEnum) {
				var vs = Enum.GetValues(t).OfType<object>().Select(e => (int)e).ToList();
				return vs.Count > 2 && vs.All(e => (int)Math.Pow(2, vs.IndexOf(e)) == e);
			}
			return false;
		}

		/// <summary>
		/// 为对象赋予随机属性，对于实现了 <see cref="ICollection{T}"/> 的对象，其 <see cref="ICollection{T}.Count"/> 将在 <see cref="MinListInstanceCount"/> 和 <see cref="MaxListInstanceCount"/> 之间。
		/// </summary>
		/// <param name="obj">生成随机属性的目标对象。</param>
		public static void InitInstance(object obj) {
			var type = obj.GetType();
			var isList = type.IsGenericType && type.GetInterfaces().Any(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(ICollection<>));
			var ps = type.GetProperties().Where(e => e.CanWrite && e.GetIndexParameters().Length == 0 && (!isList || e.Name != nameof(List<int>.Capacity)));
			ps.Where(e => e.PropertyType.Assembly == typeof(object).Assembly || e.PropertyType.IsEnum)
				.ToList().ForEach(e => e.SetValue(obj, CreateInstance(e.PropertyType)));
			ps.Where(e => e.PropertyType.Assembly != typeof(object).Assembly && !e.PropertyType.IsEnum)
				.ToList().ForEach(e => e.SetValue(obj, CreateInstance(e.PropertyType)));
			if (isList) {
				var a0type = type.GenericTypeArguments[0];
				var min = MinListInstanceCount;
				var max = MaxListInstanceCount;
				if (min <= 0) {
					var es = a0type.GetProperties().Where(e => e.PropertyType.IsEnum);
					min = es.Any() ? es.Max(e => e.PropertyType.GetFields((BindingFlags)24).Count()) : 0;
					min = min == 0 ? 2 : min;
				}
				if (max <= min) {
					max = min * min;
				}
				var listMethod = type.GetMethod(nameof(ICollection<int>.Add));
				for (var i = 0; i < rdm.Next(min, max); i++) {
					listMethod.Invoke(obj, new[] { CreateInstance(a0type) });
				}
			}
		}

		/// <summary>
		/// 类似 <see cref="Activator.CreateInstance(ActivationContext)"/>，但属性是随机的。
		/// </summary>
		/// <typeparam name="T">要创建实例的类型。</typeparam>
		/// <returns>具有随机属性的实例。</returns>
		public static T CreateInstance<T>() {
			return (T)CreateInstance(typeof(T));
		}
	}
}
