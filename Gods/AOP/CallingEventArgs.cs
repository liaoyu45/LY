using System;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	public class CallingEventArgs : EventArgs {
		public CallingEventArgs(IMethodCallMessage methods) {
			Methods = methods;
		}

		public IMethodCallMessage Methods { get; }
		public ModelBase Model { get; set; }
	}
}
