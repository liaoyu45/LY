using System;
using System.Linq;
using System.Reflection;

namespace Me.Limit {
	public class LimitIDo : World.ILimit<I> {

		public Plan[] Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int? pMax, bool done) {
			return null;
		}

		public int Desire(string t) {
			return 1;
		}

		public int Feel(string content, string tag, double value, DateTime? appearTime, int? planId) {
			throw new NotImplementedException();
		}

		public int Id { get; set; }

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
