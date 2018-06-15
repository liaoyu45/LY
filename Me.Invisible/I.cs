using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Me.Invisible {
	[Export(typeof(Me.I))]
	public class I : Me.I {
		private static Random r = new Random();

		private int avgFeeling = -1;
		public I() {
			if (avgFeeling == -1) {
				avgFeeling = Db.Using(e => e.Mines.Any() ? (int)e.Mines.Average(ee => ee.Value) : 0);
			}
		}

		public int Id { get; set; }

		int Me.I.Pay(int planId, int some) {
			if (some < 1) {
				return 0;
			}
			return Db.Using(d => {
				var p = d.Plans.Find(planId);
				if (p == null || p.GodId != Id) {
					return -2;
				}
				if (p.Done) {
					return -1;
				}
				Effort e;
				p.Efforts.Add(e = new Effort { });
				e.Real = r.Next(some);
				for (int i = 0; i < avgFeeling; i++) {
					e.Real = r.Next(e.Real, some);
				}
				if (p.Efforts.Sum(ee => ee.Real) >= p.Required) {
					p.Done = true;
					return -1;
				}
				return e.Real;
			});
		}

		int Me.I.Desire(string thing) {
			return Db.Using(d => d.Plans.Add(new Plan {
				GodId = Id,
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
			return Db.Using(e => e.Mines.Any() ? (int)e.Mines.Average(ee => ee.Value) : 0);
		}

		int Me.I.FindMyself(string name) {
			if (Id > 0) {
				return 0;
			}
			return Db.Using(d => {
				if (d.Gods.Any(e => e.Name == name)) {
					return 0;
				}
				return d.Gods.Add(new God { Name = name, Luck = name.GetHashCode() }).Luck;
			});
		}

		int Me.I.Awake(string name, int luck, string dailyContent) {
			return Db.Using(d => {
				var g = d.Gods.FirstOrDefault(e => e.Name == name && e.Luck == luck);
				if (g == null) {
					return 0;
				}
				DailyState s;
				g.DailyStates.Add(s = new DailyState { Content = dailyContent, Energy = r.Next(dailyContent.GetHashCode()) });
				return s.Energy;
			});
		}
	}
}
