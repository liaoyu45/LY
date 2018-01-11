using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Gods {

	public static class Him {
		public readonly static string MicrosoftCompany = nameof(Microsoft).ToLower();
		public readonly static string MeMySelfAndI = typeof(Him).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;

		public static bool IsOutAssembly(Assembly assembly) {
			var c = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company?.ToLower() ?? string.Empty;
			return c == MicrosoftCompany || c == MeMySelfAndI;
		}

		/// <summary>
		/// 获取一个类型的所有调用者。
		/// </summary>
		/// <param name="target">要查找的目标类型。</param>
		/// <returns>所有调用者。</returns>
		public static IEnumerable<MethodBase> GetCallers(Type target) {
			return new StackTrace(1).GetFrames().Select(f => f.GetMethod())
				.Where(t =>
					t.DeclaringType != target
					&& Any(c => c == MicrosoftCompany || c == MeMySelfAndI, t.DeclaringType.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company));
		}

		/// <summary>
		/// 返回包含自身的继承链。
		/// </summary>
		/// <param name="self">起始类，从此开始向上寻找。</param>
		/// <param name="target">目标类，终止于这里。</param>
		/// <returns>如果起始类不继承目标类，返回值的长度为空。</returns>
		public static IEnumerable<Type> GetBases(Type self, Type target) {
			if (self.IsSubclassOf(target)) {
				yield return self;
				foreach (var t in GetBases(self.BaseType, target)) {
					yield return t;
				}
			} else {
				if (self == target) {
					yield return target;
				}
			}
		}

		public static IEnumerable<T> GetAllAttributes<T>(MethodBase method) where T : Attribute {
			return method.GetCustomAttributes<T>(true).Concat(
								(from i in method.DeclaringType.GetInterfaces()
								 from m in i.GetMethods()
								 where m.Name == method.Name && false == m.GetParameters().Select(p => p.ParameterType).Except(method.GetParameters().Select(p => p.ParameterType)).Any()
								 select m).FirstOrDefault()?.GetCustomAttributes<T>(true));
		}
		public static IEnumerable<T> GetAllAttributes<T>(Type type) where T : Attribute {
			return type.GetCustomAttributes<T>(true).Concat(type.GetInterfaces().SelectMany(i => i.GetCustomAttributes<T>(true))).ToArray();
		}

		/// <summary>
		/// 将方法名称和各个参数类型的 <see cref="object.GetHashCode"/> 的累加再除以总数，以值为此方法的标识。仅保证在定义的类中几乎唯一。
		/// </summary>
		/// <param name="method">要标记的方法。</param>
		/// <param name="extra">通常为 null。</param>
		/// <returns>生成的标识。</returns>
		public static int SignMethod(MethodInfo method, object extra) {
			var ps = method.GetParameters();
			var c = ps.Length + 2;
			return method.Name.GetHashCode() / c + ps.Aggregate(0, (i, p) => i + p.GetHashCode() / c) + (extra?.GetHashCode() ?? 0) / c;
		}

		/// <summary>
		/// 查找 <see cref="SignMethod(MethodInfo, object)"/> 的返回值对应的方法。
		/// </summary>
		/// <param name="type">要查找的类型。</param>
		/// <param name="sign">原方法标识。</param>
		/// <param name="extra">通常为 null。</param>
		/// <returns><paramref name="sign"/> 对应的方法。</returns>
		public static MethodInfo GetSignedMethod(Type type, int sign, object extra) {
			return type.GetMethods().FirstOrDefault(m => SignMethod(m, extra) == sign);
		}
		public static MethodInfo GetMappedMethod(Type type, MethodInfo method, object extra) {
			return GetSignedMethod(type, SignMethod(method, extra), extra);
		}

		/// <summary>
		/// 对于无 out 参数的方法，可免去类型声明，如：T t; try { t = func(); } catch { t = default(T); }，现可使用：var obj = Him.TryGet(func)。
		/// </summary>
		/// <typeparam name="T"><paramref name="func"/> 的返回值类型。</typeparam>
		/// <param name="func">可能发生异常的函数。</param>
		/// <param name="ifError">当发生异常时作为替代的返回值。</param>
		/// <returns><paramref name="func"/> 返回值类型的实例。</returns>
		public static T TryGet<T>(Func<T> func, T ifError = default(T)) {
			try {
				return func();
			} catch {
				return ifError;
			}
		}

		/// <summary>
		/// 类似于 <see cref="List{T}.ForEach(Action{T})"/>。
		/// </summary>
		public static void ForEach<T>(Action<T> action, T t, params T[] list) {
			action(t);
			foreach (var tt in list) {
				action(tt);
			}
		}

		/// <summary>
		/// 类似于 <see cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>。
		/// </summary>
		public static bool Any<T>(Func<T, bool> func, T t, params T[] list) {
			if (func(t)) {
				return true;
			}
			foreach (var tt in list) {
				if (func(tt)) {
					return true;
				}
			}
			return false;
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