using System;

namespace Gods.AOP {
	public class ValidatorAttribute : Attribute {
		public ValidatorAttribute(Type validatorType) {
			ValidatorType = validatorType;
		}

		public Type ValidatorType { get; }
	}
}