using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Me.World {
	[Export(typeof(Soul))]
	public class I : Soul, Me.I {
		private static Random r = new Random();

		private static int avgFeeling = -1;
		public I() {
			if (avgFeeling == -1) {
				ResetAvgFeeling();
				avgFeeling = Math.Max(avgFeeling, 0);
			}
		}

		private void ResetAvgFeeling() {
			if (Db.Instance.Mines.Any()) {
				avgFeeling = (int)Db.Instance.Mines.Average(e => e.Value);
			}
		}

		int Soul.Pay(int planId, int some) {
			var plan = Db.Instance.Plans.Find(planId);
			if (plan.Done) {
				return 0;
			}
			Effort e;
			plan.Efforts.Add(e = new Effort { Payed = some });
			Db.Instance.SaveChanges();
			e.Real = r.Next(some);
			for (int i = 0; i < avgFeeling; i++) {
				e.Real = r.Next(e.Real);
			}
			Db.Instance.SaveChanges();
			return e.Real;
		}

		int Soul.Desire(string thing) {
			var n = DateTime.Now;
			var p = Db.Instance.Plans.Add(new Plan {
				Content = thing,
				AppearTime = n,
				Required = r.Next(thing.GetHashCode() / 2 + n.GetHashCode() / 2, int.MaxValue),
			});
			Db.Instance.SaveChanges();
			return p.Required;
		}

		int Soul.Feel(string content, string tag, double value, long last, int? planId) {
			Db.Instance.Mines.Add(new Mine {
				Content = content,
				Tag = tag,
				Value = value,
				Last = last,
				PlanId = planId
			});
			return 1;
		}

		int Soul.GiveUp(int planId) {
			throw new NotImplementedException();
		}

		Plan[] Soul.Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int? pMax, bool done) {
			return null;
		}

		int Soul.MyAverageFeeling() {
			return avgFeeling;
		}

		void Soul.Suicide() {
			Db.Instance.Database.Delete();
			Db.Instance.Database.Create();
		}
	}
}
