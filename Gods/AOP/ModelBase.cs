using System;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	[AOP]
	public class ModelBase : ContextBoundObject {
		public static Action<ModelBase> NewModel;

		public event EventHandler<CallingEventArgs> Calling;

		internal virtual void NotifyMethod(IMethodCallMessage methods) {
			try {
				Calling?.Invoke(this, new CallingEventArgs(methods));
			} catch {
			}
		}
	}

}
