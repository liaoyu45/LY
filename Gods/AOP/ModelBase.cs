using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
	[AOP]
	public abstract class ModelBase : ContextBoundObject { }

	public class Model : ModelBase {
		public Model() {
			var t = ValidatorType;
			if (t == null) {
				return;
			}
			typeof(ValidatorExtensions)
				.GetMethod(nameof(ValidatorExtensions.Validate), BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(GetType())
				.Invoke(null, new[] { Activator.CreateInstance(typeof(GenericValidator<>).MakeGenericType(t)) });
		}
		protected internal virtual MethodInfo GetValidator(Type type, MethodBase name) {
			return type.GetMethod(Him.GetAllAttributes<TargetMethodAttribute>(name).FirstOrDefault()?.Name ?? string.Empty);
		}

		protected virtual Type ValidatorType {
			get {
				var type = Him.GetAllAttributes<TargetTypeAttribute>(GetType()).FirstOrDefault()?.ValidatorType;
				if (type == null) {
					return null;
				}
				if (cache.ContainsKey(type.GUID)) {
					return cache[type.GUID];
				}
				if (type.IsInterface && Directory.Exists(Folder)) {
					var pre = type.Assembly.FullName.Split(',')[0] + '.';
					return cache[type.GUID] = Directory.GetFiles(Folder, "*.dll")
						.Select(e => Him.TryGet(() => Assembly.LoadFrom(e)))
						.Where(ass => ass?.FullName.Split(',')[0].StartsWith(pre) == true)
						.OrderBy(ae => ae.FullName)
						.LastOrDefault()?.ExportedTypes
						.FirstOrDefault(t => t.GetInterfaces().Contains(type));
				}
				return type;
			}
		}
		public virtual string Folder { get; } = nameof(Gods);

		private static Dictionary<Guid, Type> cache = new Dictionary<Guid, Type>();
	}
}
