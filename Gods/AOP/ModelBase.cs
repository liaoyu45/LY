using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
	[AOP]
	public abstract class ModelBase : ContextBoundObject {
		public ModelBase() {
			if (ValidatorType == null) {
				return;
			}
			typeof(ValidatorExtensions).GetMethod(nameof(ValidatorExtensions.Validate)).MakeGenericMethod(GetType()).Invoke(null, new[] { Activator.CreateInstance(typeof(GenericValidator<>).MakeGenericType(ValidatorType)) });
		}
		protected virtual Type ValidatorType => Him.GetAllAttribute<TargetTypeAttribute>(GetType()).FirstOrDefault()?.ValidatorType;

		protected internal virtual string GetValidator(MethodBase name) {
			return Him.GetAllAttribute<TargetMethodAttribute>(name).FirstOrDefault().Name;
		}
	}
	public class Model : ModelBase, IModelFactory {
		private Type type;
		protected override Type ValidatorType {
			get {
				return type ?? (type = ((IModelFactory)this).GetType(base.ValidatorType));
			}
		}
		public virtual string Folder { get; } = nameof(Gods);
		private static Dictionary<Guid, Type> cache = new Dictionary<Guid, Type>();

		Type IModelFactory.GetType(Type type) {
			if (cache.ContainsKey(type.GUID)) {
				return cache[type.GUID];
			}
			if (type.IsInterface || type.IsAbstract) {
				var pre = type.Assembly.FullName.Split(',')[0] + '.';
				return cache[type.GUID] = Directory.GetFiles(Folder)
					.Select(e => Him.TryGet(() => Assembly.LoadFile(e)))
					.Where(ass => ass?.FullName.Split(',')[0].StartsWith(pre) == true)
					.OrderBy(ae => ae.FullName)
					.LastOrDefault()?.ExportedTypes.
					FirstOrDefault(t => t.GetInterfaces().Contains(type));
			}
			return type;
		}
	}
	public interface IModelFactory {
		Type GetType(Type type);
	}
}
