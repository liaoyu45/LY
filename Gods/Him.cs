using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gods {
	public static partial class Him {
		public readonly static string MicrosoftCompany = nameof(Microsoft).ToLower();
		public readonly static string MeMySelfAndI = typeof(Him).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;

		public static bool IsOutAssembly(Assembly assembly) {
			var c = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company?.ToLower() ?? string.Empty;
			return c.StartsWith(MicrosoftCompany) || c.StartsWith(MeMySelfAndI);
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

		public static IEnumerable<Type> GetBases(Type self) {
			return GetBases(self, typeof(object));
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

		public static IEnumerable<T> GetAllAttributes<T>(MethodInfo method) where T : Attribute {
			return (method?.GetCustomAttributes<T>(true) ?? Enumerable.Empty<T>()).Concat(
								(from i in method?.DeclaringType.GetInterfaces() ?? Enumerable.Empty<Type>()
								 from m in i.GetMethods()
								 where SignMethod(m) == SignMethod(method)
								 select m).FirstOrDefault()?.GetCustomAttributes<T>(true) ?? Enumerable.Empty<T>());
		}
		public static IEnumerable<T> GetAllAttributes<T>(Type type) where T : Attribute {
			return (type.GetCustomAttributes<T>(true) ?? Enumerable.Empty<T>()).Concat(type.GetInterfaces().SelectMany(i => i.GetCustomAttributes<T>(true) ?? Enumerable.Empty<T>())).ToArray();
		}

		/// <summary>
		/// 将函数名称和参数的 <see cref="object.GetHashCode"/> 的平均值做为此函数的标识。仅保证在定义的类中几乎唯一。
		/// </summary>
		/// <param name="method">要标记的函数。</param>
		/// <returns>生成的标识。</returns>
		public static int SignMethod(MethodInfo method) {
			return (method.Name.GetHashCode()) + method.GetParameters().Aggregate(0, (i, p) => i + (p.ParameterType.GetHashCode()));
		}

		/// <summary>
		/// 查找对应的函数。
		/// </summary>
		/// <param name="type">要查找的类型。</param>
		/// <param name="sign">原函数标识。</param>
		/// <param name="extra">通常为 null。</param>
		/// <returns>对应的函数。</returns>
		public static MethodInfo GetSignedMethod(Type type, int sign) {
			return type?.GetMethods().Where(e => !e.IsSpecialName).Concat(type.GetInterfaces().SelectMany(e => e.GetMethods().Where(ee => !ee.IsSpecialName))).FirstOrDefault(m => SignMethod(m) == sign);
		}

		/// <summary>
		/// 在 <paramref name="type"/> 中找到和 <paramref name="method"/> 函数具有相同签名的函数。
		/// </summary>
		/// <param name="type">目标函数所在的类型。</param>
		/// <param name="method">目标函数。</param>
		/// <returns>和目标函数签名相同的函数。</returns>
		public static MethodInfo GetMappedMethod(Type type, MethodInfo method) {
			return GetSignedMethod(type, SignMethod(method));
		}

		/// <summary>
		/// 在获取可能发生异常的函数的返回值时，免去类型声明。如：T t; try { t = func(); } catch { t = default(T); }，现可使用：var obj = Him.TryGet(func)。
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
		public static Dictionary<string, object> CopyTo(this object fromA, object toB, params string[] excludes) =>
			CopyTo(fromA, toB, true, excludes);
		public static Dictionary<string, object> CopyTo(this object fromA, object toB, bool contains, params string[] props) {
			if (fromA == null || toB == null) {
				return null;
			}
			var wontCheck = props.Length == 0;
			var r = new Dictionary<string, object>();
			toB.GetType().GetProperties()
				.Where(p => p.CanWrite && (wontCheck || props.Contains(p.Name) == contains))
				.ToList().ForEach(p => {
					var va = fromA.GetType().GetProperty(p.Name)?.GetValue(fromA);
					if (va?.GetType() == p.PropertyType) {
						p.SetValue(toB, va);
						r[p.Name] = va;
					}
				});
			return r;
		}

		/// <summary>
		/// 在 <paramref name="folder"/> 中找到一个接口（<paramref name="type"/>）的实现类。
		/// </summary>
		/// <param name="type">一个接口类型。</param>
		/// <param name="folder">实现类的程序集所在的文件夹。</param>
		/// <returns>一个实现了指定接口类型的实例类型列表。</returns>
		public static IEnumerable<Type> FindImplements(Type tagInterface, string folder) {
			return Directory.GetFiles(folder, "*.dll").SelectMany(a => {
				try {
					return Assembly.LoadFrom(a)?.ExportedTypes.Where(e => e.GetInterfaces().Contains(tagInterface));
				} catch {
					return Enumerable.Empty<Type>();
				}
			});
		}

		public static bool SameQueue(IEnumerable a, IEnumerable b) {
			if (a == null) {
				return b == null;
			} else if (b == null) {
				return false;
			}
			var aa = a.GetEnumerator();
			var bb = b.GetEnumerator();
			while (aa.MoveNext()) {
				if (!bb.MoveNext() || aa.Current != bb.Current) {
					return false;
				}
			}
			return !bb.MoveNext();
		}
	}
}