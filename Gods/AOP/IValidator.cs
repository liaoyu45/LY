using System.Reflection;

namespace Gods.AOP {
    public interface IValidator {
        Modes Mode { get; }
        bool Inheritable { get; }

        bool Sibling(ModelBase target, MethodBase calling, MethodBase caller);
        bool Foreigner(ModelBase obj, MethodBase calling, MethodBase caller);
    }
}