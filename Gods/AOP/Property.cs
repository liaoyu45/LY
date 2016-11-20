using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
    class Property : IContextProperty, IContributeServerContextSink {
        #region IContextProperty
        public Property() {
        }
        public void Freeze(Context newContext) {
        }

        public bool IsNewContextOK(Context newCtx) {
            return true;
        }

        public string Name {
            get { return typeof(Property).Name; }
        }
        #endregion
        #region IContributeObjectSink
        public IMessageSink GetServerContextSink(IMessageSink nextSink) {
            return new Sink(nextSink);
        }
        #endregion
    }
}