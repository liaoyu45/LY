using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gods.Web {
	public static partial class Him {
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
	}
}
