using System;
using System.Linq;
using System.Threading;

namespace Me.Invisible {
	public class I : Me.I {
		private static Random r = new Random();
		private static int Random(object n) {
			return r.Next(Math.Abs(n.GetHashCode()));
		}

		public int Id { get; set; }

		int Me.I.Pay(int planId, int some) {
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
			return Universe.Using(d => d.Possessions.Any(e => e.GodId == Id) ? (int)d.Possessions.Where(e => e.GodId == Id).Average(ee => ee.Value) : 0);
		}

		int Me.I.FindMyself(string name) {
			return Universe.Using(d => {
				if (d.Gods.Any(e => e.Name == name)) {
					return 0;
				}
				var luck = Random(name);
				d.Gods.Add(new God { Name = name, Luck = luck });
				new Thread(() => {
					Thread.Sleep(11111);
					Universe.Using(dd => dd.Gods.Remove(dd.Gods.First(e => e.Luck == luck)));
				}).Start();
				return luck;
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
				g.DailyStates.Add(s = new DailyState {
					Content = dailyContent,
					Energy = Random(dailyContent)
				});
				return s.Energy;
			});
		}
	}
}
