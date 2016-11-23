using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
    class Sink : IMessageSink {
        private SinkHelper helper;

        public Sink(IMessageSink nextSink) {
            this.NextSink = nextSink;
        }

        public IMessageSink NextSink { get; }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink) {
            return null;
        }

        public IMessage SyncProcessMessage(IMessage msg) {
            var method = msg as IMethodMessage;
            if (method.MethodBase.IsConstructor) {
                return createHelper(msg);
            }
            this.helper?.Analysis(method.MethodBase);
            return this.NextSink.SyncProcessMessage(msg);
        }

        private IMessage createHelper(IMessage msg) {
            var retMsg = this.NextSink.SyncProcessMessage(msg);
            var model = RemotingServices.Unmarshal((retMsg as IMethodReturnMessage).ReturnValue as ObjRef);
            var type = model.GetType();
            if (ModelExtensions.GetValidators(type).GetEnumerator().MoveNext()) {
                this.helper = new SinkHelper(model as ModelBase);
            }
            return retMsg;
        }
    }
}