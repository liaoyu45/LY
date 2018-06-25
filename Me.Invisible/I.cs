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

		int Me.I.Desire(string thing, bool test) {
			var h = thing?.GetHashCode() ?? 0;
			if (test) {
				return h;
			}
			var a = Awake(null);
			if (a == null) {
				return 0;
			}
			h = (int)(h / (Math.Log(a.Content.Length) / Math.Log(2)));
			for (var i = 0; i < a.Content.Length; i++) {
				h = r.Next(h);
			}
			Universe.Using(d => d.Plans.Add(new Plan {
				GodId = Id,
				Content = thing,
				Required = h,
			}));
			return h;
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

		string Me.I.FindMyself(string name, string password) {
			if (Id > 0) {
				return Universe.Using(d => d.Gods.Find(Id)?.Name);
			} else if (name?.Trim().Any() != true || password?.Trim().Any() != true) {
				return null;
			}
			return Universe.Using(d => {
				var g = d.Gods.FirstOrDefault(e => e.Name == name);
				if (g == null) {
					d.Gods.Add(g = new God { Name = name, Password = password });
					d.SaveChanges();
					return name;
				}
				if (g.Password != password) {
					return null;
				}
				Id = g.Id;
				return name;
			});
		}

		void Me.I.Leave() {
			Id = 0;
		}

		public DailyState Awake(string dailyContent) {
			return Universe.Using(d => {
				var god = d.Gods.Find(Id);
				var today = DateTime.Now.Date;
				var tomorrow = DateTime.Now.AddDays(1).Date;
				var s = d.DailyStates.FirstOrDefault(e => e.GodId == Id && e.AppearTime >= today && e.AppearTime < tomorrow);
				if (dailyContent == null) {
					return s;
				}
				if (s == null) {
					god.DailyStates.Add(s = new DailyState {
						Content = dailyContent,
						Energy = Random(dailyContent)
					});
				}
				return s;
			});
		}
	}
}
