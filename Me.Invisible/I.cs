using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Me.Invisible {
	public class I : Me.I {
		public int Id { get; set; }
		public Dictionary<int, Expression<Func<Plan, bool>>> Query { get; set; } = new Dictionary<int, Expression<Func<Plan, bool>>>();

		private DateTime today = DateTime.Now.Date;
		private DateTime tomorrow = DateTime.Now.AddDays(1).Date;

		int Me.I.Pay(int planId, string content) {
			return Universe.Using(d => {
				var p = d.Plans.Find(planId);
				if (p == null || p.GodId != Id) {
					return 0;
				}
				if (p.Done) {
					return 0;
				}
				var payed = GetHashCode(content);
				var r = new Random();
				for (var i = 0; i < p.Content.Length; i++) {
					payed = r.Next(payed);
				}
				payed *= content.Length;
				var s = TodayState(d);
				if (s == null || s.Energy < payed) {
					return 0;
				}
				s.Energy -= payed;
				p.Efforts.Add(new Effort { Content = content, Payed = payed });
				p.Current += payed;
				return payed;
			});
		}

		int Me.I.Desire(string thing, bool test) {
			var h = GetHashCode(thing);
			if (test) {
				return h;
			}
			Universe.Using(d => d.Plans.Add(new Plan {
				GodId = Id,
				Content = thing,
				Required = h,
			}));
			return h;
		}

		void Me.I.Feel(string content, string tag, int? planId, int value) {
			Universe.Using(d => {
				d.Possessions.Add(new Possession {
					Content = content,
					Tag = tag,
					Value = value,
					PlanId = planId,
					GodId = Id
				});
			});
		}

		void Me.I.GiveUp(int planId, bool forever) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId);
				if (p == null) {
					return;
				}
				if (forever) {
					d.Plans.Remove(p);
				} else {
					p.Abandoned = true;
				}
			});
		}

		QueryData Me.I.ArrangePrepare(DateTime? start, DateTime? end, int? minRequired, int? maxRequired, int? minValue, int? maxValue, bool? done, bool? abandoned) {
			var id = nameof(Me.I.ArrangePrepare).GetHashCode();
			Query[id] = e =>
				e.GodId == Id
				&& (e.AppearTime > (start ?? DateTime.MinValue))
				&& (e.AppearTime < (end ?? DateTime.MaxValue))
				&& e.Required > (minRequired ?? -1)
				&& e.Required < (maxRequired ?? int.MaxValue)
				&& e.Current > (minValue ?? -1)
				&& e.Current < (maxValue ?? int.MaxValue)
				&& (!done.HasValue || e.Finished.HasValue)
				&& (!abandoned.HasValue || e.Abandoned == abandoned.Value);
			return new QueryData {
				Id = id, Total = Universe.Using(d => {
					d.Database.Log = ee => Console.WriteLine(ee);
					return d.Plans.Count(Query[id]);
				})
			};
		}

		Plan[] Me.I.ArrangeQuery(int query, int start, int end) {
			Expression<Func<Plan, bool>> value;
			if (Query.TryGetValue(query, out value)) {
				return Universe.Using(d => d.Plans.Include(e => e.Efforts).Where(value).ToArray());
			}
			return Array.Empty<Plan>();
		}

		public int MyFeelingsCount() {
			return Universe.Using(d => d.Possessions.Count(e => e.GodId == Id));
		}

		string Me.I.FindMyself(string name, string password) {
			if (Id > 0) {
				return Universe.Using(d => d.Gods.Find(Id)?.Name);
			} else if (name?.Any() != true || password?.Any() != true) {
				return null;
			}
			return Universe.Using(d => {
				var g = d.Gods.FirstOrDefault(e => e.Name == name);
				if (g == null) {
					d.Gods.Add(g = new God { Name = name, Password = password });
					d.SaveChanges();
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

		public DailyState WakeUp(string dailyContent) {
			return Universe.Using(d => {
				var god = Id <= 0 ? null : d.Gods.Find(Id);
				if (god == null) {
					return null;
				}
				var s = TodayState(d);
				if (dailyContent == null) {
					return s;
				}
				if (s == null) {
					var r = new Random();
					var v = r.Next();
					var fc = Math.Log(dailyContent.Length);
					for (var i = 0; i < fc; i++) {
						v = r.Next(v, int.MaxValue);
					}
					god.DailyStates.Add(s = new DailyState {
						Content = dailyContent,
						Energy = v
					});
				}
				return s;
			});
		}

		private DailyState TodayState(Universe d) {
			return Id > 0 ? d.DailyStates.FirstOrDefault(e => e.GodId == Id && e.AppearTime >= today && e.AppearTime < tomorrow) : null;
		}

		private int GetHashCode(object o) {
			return Math.Abs(o?.GetHashCode() ?? 0);
		}

		void Me.I.Resume(int planId) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.Id == planId && e.GodId == Id && e.Abandoned);
				if (p != null) {
					p.Abandoned = false;
				}
			});
		}
	}
}
