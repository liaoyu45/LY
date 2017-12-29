using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	public class AOPException : Exception {
		public ModelBase Target { get; }
		public MethodBase Caller { get; }
		public IMethodCallMessage Calling { get; }

		internal AOPException(ModelBase target, IMethodCallMessage calling, MethodBase badCallers, Exception inner) : base("see" + nameof(InnerException), inner) {
			Target = target;
			Calling = calling;
			Caller = badCallers;
		}
	}
}