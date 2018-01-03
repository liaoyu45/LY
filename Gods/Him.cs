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

		public static IEnumerable<T> GetAllAttribute<T>(MethodBase method) where T : Attribute {
			return method.GetCustomAttributes<T>(true).Concat(
								(from i in method.DeclaringType.GetInterfaces()
								 from m in i.GetMethods()
								 where m.Name == method.Name && false == m.GetParameters().Select(p => p.ParameterType).Except(method.GetParameters().Select(p => p.ParameterType)).Any()
								 select m).FirstOrDefault()?.GetCustomAttributes<T>(true));
		}
		public static IEnumerable<T> GetAllAttribute<T>(Type type) where T : Attribute {
			return type.GetCustomAttributes<T>(true).Concat(type.GetInterfaces().SelectMany(i => i.GetCustomAttributes<T>(true)));
		}

		public static Dictionary<int, string> TryAll(Action action, params Action[] actions) {
			var dict = new Dictionary<int, string>();
			try {
				action();
			} catch (Exception e) {
				dict.Add(0, e.Message);
			}
			for (var i = 0; i < actions.Length; i++) {
				try {
					actions[i]();
				} catch (Exception e) {
					dict.Add(i + 1, e.Message);
				}
			}
			return dict;
		}

		/// <summary>
		/// 用于当 <paramref name="func"/> 可能发生异常时。可免去类型声明，如：T t; try { t = func(); } catch { t = default(T); }，现可使用：var obj = Him.TryGet(func)。
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
	}
}