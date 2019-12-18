using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	class Sink : IMessageSink {
		private ModelBase modelBase;

		public IMessageSink NextSink { get; set; }

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink) => null;

		public IMessage SyncProcessMessage(IMessage msg) {
			var method = msg as IMethodCallMessage;
			var r = NextSink.SyncProcessMessage(msg);
			if (method.MethodBase.IsConstructor) {
				modelBase = RemotingServices.Unmarshal((r as IMethodReturnMessage).ReturnValue as ObjRef) as ModelBase;
				try {
					ModelBase.NewModel?.Invoke(modelBase);
				} catch {
				}
			} else {
				modelBase.NotifyMethod(method);
			}
			return r;
		}
	}
}