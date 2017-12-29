using System;
using System.Linq;

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
	}
	public abstract class ModelBaseFactory : ModelBase {
		protected sealed override Type ValidatorType {
			get {
				return Factory.GetType(base.ValidatorType);
			}
		}
		protected abstract IModelFactory Factory { get; }
	}
	public interface IModelFactory {
		Type GetType(Type type);
	}
}
