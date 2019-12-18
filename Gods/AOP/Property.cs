using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	class Property : IContextProperty, IContributeServerContextSink {
		public void Freeze(Context newContext) { }
		public bool IsNewContextOK(Context newCtx) => true;
		public string Name => nameof(Property);

		public IMessageSink GetServerContextSink(IMessageSink nextSink) => new Sink { NextSink = nextSink };
	}
}