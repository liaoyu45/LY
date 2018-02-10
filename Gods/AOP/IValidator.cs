using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	public interface IValidator {
		bool Inheritable { get; }

		void Validate(ModelBase target, IMethodCallMessage calling, MethodBase caller);
	}

	class GenericValidator<T> : IValidator {
		public bool Inheritable => true;

		void IValidator.Validate(ModelBase target, IMethodCallMessage calling, MethodBase caller) {
			var t = (Model)target;
			var m = t.GetValidator(typeof(T), calling.MethodBase);
			try {
				var model = Activator.CreateInstance<T>();
				if (model is Model) {
					var mm = model as Model;
					mm.Values = t.Values;
					Mapper.Invoke(mm, m);
				}
			} catch (Exception e) {
				throw e.InnerException;
			}
		}
	}
}