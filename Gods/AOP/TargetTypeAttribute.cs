using System;

namespace Gods.AOP {
	public class TargetTypeAttribute : Attribute {
		public TargetTypeAttribute(Type validatorType) {
			ValidatorType = validatorType;
		}

		public Type ValidatorType { get; }
	}
}