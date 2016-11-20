using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace Gods.AOP {
    class AOPAttribute : ContextAttribute {
        public AOPAttribute()
            : base(nameof(AOPAttribute)) {

        }
        public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg) {
            ctorMsg.ContextProperties.Add(new Property());
        }
    }
}