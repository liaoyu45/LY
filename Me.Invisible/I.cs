using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Me.Invisible {
	[Export(typeof(Me.I))]
	public class I : Me.I {
		private static Random r = new Random();

		private static int avgFeeling = -1;
		public I() {
			if (avgFeeling == -1) {
				avgFeeling = Db.Using(e => e.Mines.Any() ? (int)e.Mines.Average(ee => ee.Value) : 0);
			}
		}

		int Me.I.Pay(int planId, int some) {
			var plan = Db.Instance.Plans.Find(planId);
			if (plan.Done) {
				return 0;
			}
			Effort e;
			plan.Efforts.Add(e = new Effort { Payed = some });
			Db.Instance.SaveChanges();
			e.Real = r.Next(some);
			for (int i = 0; i < avgFeeling; i++) {
				e.Real = r.Next(e.Real, some);
			}
			Db.Instance.SaveChanges();
			return e.Real;
		}

		int Me.I.Desire(string thing) {
			return Db.Using(d => d.Plans.Add(new Plan {
				Content = thing,
				Required = r.Next(thing.GetHashCode(), int.MaxValue),
			})).Required;
		}

		int Me.I.Feel(string content, string tag, double value, DateTime? appearTime, int? planId) {
			Db.Using(d => d.Mines.Add(new Mine {
				Content = content,
				AppearTime = appearTime ?? DateTime.Now,
				Tag = tag,
				Value = value,
				PlanId = planId
			}));
			return 1;
		}

		int Me.I.GiveUp(int planId) {
			throw new NotImplementedException();
		}

		Plan[] Me.I.Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int? pMax, bool done) {
			return null;
		}

		int Me.I.MyAverageFeeling() {
			return avgFeeling;
		}

		int Me.I.Awake(string name) {
			Db.Using(d => d.Gods.Add(new God { Name = name, Luck = name.GetHashCode() }));
			return name.GetHashCode();
		}
	}
}
