using System;
using System.Linq;

namespace Gods.Web {
	public class TypeCache {
		public Type Declare { get; set; }
		public Type Implement { get; set; }
		private bool ever;//TODO:reload maybe

		public TypeCache(Type declare) {
			Declare = declare;
			if (!declare.IsAbstract) {
				Implement = declare;
			}
		}
		public Type GetImplement() {
			if (ever) {
				return Implement;
			}
			ever = true;
			return Implement = Him.FindImplements(Declare, Him.his.Implements).FirstOrDefault(e => !e.IsAbstract && e.GetConstructors().Any(ee => ee.GetParameters().Length == 0)) ?? Declare;
		}

		public override int GetHashCode() {
			return Declare.FullName.GetHashCode();
		}

		public override bool Equals(object obj) {
			if (obj?.GetType() != GetType()) {
				return false;
			}
			return (obj as TypeCache).GetHashCode() == GetHashCode();
		}
	}
}
