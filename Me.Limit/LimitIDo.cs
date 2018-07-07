using System;

namespace Me.Limit {
	public class LimitIDo : World.ILimit<I> {
		public int Id { get; set; }

		public string GiveUp(int planId, bool forever) {
			if (forever) {
				return "Please, do not give up.";
			}
			return null;
		}
		public object[] QueryPlans(DateTime? start, DateTime? end, bool? done, bool? abandoned) {
			if (start == DateTime.MinValue) {
				start = null;
			}
			if (end == DateTime.MinValue) {
				end = null;
			}
			if (start.HasValue && end < start) {
				end = start.Value.AddDays(1);
			}
			return new object[] { start, end, done, abandoned };
		}
	}
}
