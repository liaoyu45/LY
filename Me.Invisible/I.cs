using System;
using System.Linq;

namespace Me.Invisible {
	public class I : Me.I {
		private static Random r = new Random();
		private static int Random(int n) {
			return r.Next(Math.Abs(n));
		}

		public int Id { get; set; }

		int Me.I.Pay(int planId, int some) {
			if (some < 1) {
				return 0;
			}
			return Universe.Using(d => {
				var p = d.Plans.Find(planId);
				if (p == null || p.GodId != Id) {
					return -2;
				}
				if (p.Done) {
					return -1;
				}
				Effort e;
				p.Efforts.Add(e = new Effort { });
				e.Real = Random(some);
				var v = MyAverageFeeling();
				for (int i = 0; i < v; i++) {
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
			return Universe.Using(d => d.Plans.Add(new Plan {
				GodId = Id,
				Content = thing,
				Required = r.Next(thing.GetHashCode(), int.MaxValue),
			})).Required;
		}

		int Me.I.Feel(string content, string tag, double value, DateTime? appearTime, int? planId) {
			Universe.Using(d => d.Possessions.Add(new Possession {
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

		public int MyAverageFeeling() {
			return Universe.Using(e => e.Possessions.Any() ? (int)e.Possessions.Average(ee => ee.Value) : 0);
		}

		int Me.I.FindMyself(string name) {
			return Universe.Using(d => {
				if (d.Gods.Any(e => e.Name == name)) {
					return 0;
				}
				return d.Gods.Add(new God { Name = name, Luck = Random(Math.Abs(name.GetHashCode())) }).Luck;
			});
		}

		int Me.I.Awake(string name, string dailyContent) {
			return Universe.Using(d => {
				var g = d.Gods.FirstOrDefault(e => e.Name == name);
				if (g == null) {
					return 0;
				}
				Id = g.Id;
				DailyState s;
				g.DailyStates.Add(s = new DailyState { Content = dailyContent, Energy = Random(dailyContent.GetHashCode()) });
				return s.Energy;
			});
		}
	}
}
