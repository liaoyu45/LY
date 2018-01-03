using System;
using System.Linq;
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
			var obj = Activator.CreateInstance<T>();
			try {
				typeof(T).GetMethod(target.GetValidator(calling.MethodBase))?.Invoke(obj, null);
			} catch (Exception e) {
				throw e.InnerException;
			}
		}
	}
}