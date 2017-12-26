using System.Reflection;

namespace Gods.AOP {
	public interface IValidator {
		Modes Mode { get; }
		bool Inheritable { get; }

		bool Validate(ModelBase target, MethodBase calling, MethodBase caller);
	}
	public abstract class ValidatorBase : IValidator {
		public abstract Modes Mode { get; }

		public virtual bool Inheritable => true;

		public abstract bool Validate(ModelBase target, MethodBase calling, MethodBase caller);
	}
	public abstract class ForeignValidator : ValidatorBase {
		public override Modes Mode => Modes.Foreigner;
	}
	public abstract class SiblingValidator : ValidatorBase {
		public override Modes Mode => Modes.Sibling;
	}
}