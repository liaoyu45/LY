using System;
using System.Reflection;

namespace Me.Limit {
	public class LimitIDo : World.ILimit<I> {
		public LimitIDo(I i, MethodInfo m) {
			if (m.Name == nameof(I.Awake) || m.Name == nameof(I.FindMyself)) {
				if (i.Id > 0) {
					throw null;
				}
			} else {
				if (i.Id <= 0) {
					throw null;
				}
			}
		}

		public int Id {
			get {
				throw new NotImplementedException();
			}

			set {
				throw new NotImplementedException();
			}
		}

		public Plan[] Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int? pMax, bool done) {
			return null;
		}

		public void Awake() {
		}

		public int Desire(string t) {
			return 1;
		}

		public int Feel(string content, string tag, double value, DateTime? appearTime, int? planId) {
			throw new NotImplementedException();
		}

		public void FindMyself(string name) {
		}

		public int GiveUp(int planId) {
			throw new NotImplementedException();
		}

		public int MyAverageFeeling() {
			throw new NotImplementedException();
		}

		public int Pay(int planId, int some) {
			throw new NotImplementedException();
		}
	}
}
