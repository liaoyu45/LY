using System;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
	[AOP]
	public abstract class ModelBase : ContextBoundObject {
		public ModelBase() {
			var t = GetType();
			t.GetCustomAttributes<ValidatorAttribute>().ToList().ForEach(a =>
				ModelExtensions.AddValidator(t, Activator.CreateInstance(a.ValidatorType) as IValidator));
		}
	}
}
