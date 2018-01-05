using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
	[AOP]
	public abstract class ModelBase : ContextBoundObject { }

	public class Model : ModelBase, IModelFactory {
		public Model() {
			var t = ValidatorType;
			if (t == null) {
				return;
			}
			var v = Activator.CreateInstance(
						typeof(GenericValidator<>).MakeGenericType(t));
			typeof(ValidatorExtensions)
				.GetMethod(nameof(ValidatorExtensions.Validate))
				.MakeGenericMethod(GetType())
				.Invoke(null, new[] {
					v
				});
		}
		protected internal virtual string GetValidator(MethodBase name) {
			return Him.GetAllAttribute<TargetMethodAttribute>(name).FirstOrDefault().Name;
		}

		private bool ever;
		private Type type;
		protected virtual Type ValidatorType {
			get {
				if (ever) {
					return type;
				}
				ever = true;
				var a = Him.GetAllAttribute<TargetTypeAttribute>(GetType()).FirstOrDefault();
				if (a == null) {
					return null;
				}
				return type = ((IModelFactory)this).GetType(a.ValidatorType);
			}
		}
		public virtual string Folder { get; } = nameof(Gods);
		private static Dictionary<Guid, Type> cache = new Dictionary<Guid, Type>();
		protected internal virtual void DealTarget(object obj) { }
		protected internal virtual object[] InvokeParameters(MethodBase method) {
			return null;
		}
		Type IModelFactory.GetType(Type type) {
			if (cache.ContainsKey(type.GUID)) {
				return cache[type.GUID];
			}
			if (type.IsInterface || type.IsAbstract) {
				var pre = type.Assembly.FullName.Split(',')[0] + '.';
				return cache[type.GUID] = Directory.GetFiles(Folder, "*.dll")
					.Select(e => Him.TryGet(() => Assembly.LoadFrom(e)))
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
