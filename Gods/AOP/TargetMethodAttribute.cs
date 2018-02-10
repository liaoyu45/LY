using System;

namespace Gods.AOP {
	public class TargetMethodAttribute : Attribute {
		public TargetMethodAttribute(string name) {
			Name = name;
		}

		public string Name { get; }
	}
}
