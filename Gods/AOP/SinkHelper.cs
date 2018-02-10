using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Gods.AOP {
	class SinkHelper {
		internal SinkHelper(ModelBase m) {
			model = m;
			type = m.GetType();
		}

		private ModelBase model;
		private Type type;

		internal void Analysis(IMethodCallMessage calling) {
			var vs = ValidatorExtensions.GetValidators(type);
			var caller = Him.GetCallers(type).First();
			foreach (var item in vs) {
				try {
					item.Validate(model, calling, caller);
				} catch (Exception e) {
					throw new AOPException(model, calling, caller, e);
				}
			}
		}
	}
}