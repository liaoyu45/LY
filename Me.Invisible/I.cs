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

		void Me.I.Pay(int planId, string content) {
			Universe.Using(d => {
				var p = d.Plans.Find(planId);
				if (p.DoneTime.HasValue) {
					throw new Exception(p.Content + "-已完成");
				}
				var g = d.Gods.Find(Id);
				var s = TodayState(d);
				Effort e;
				p.Efforts.Add(e = new Effort { Content = content });
				if (p.DoneTime.HasValue) {
					p.DoneTime = DateTime.Now;
					g.Luck += new Random().NextDouble() * (1 - g.Luck);
				}
			});
		}

		void Me.I.Desire(string thing) {
			Universe.Using(d => {
				var s = TodayState(d);
				var r = GetHashCode(thing);
				d.Plans.Add(new Plan {
					GodId = Id,
					Content = thing
				});
			});
		}

		void Me.I.Feel(string content, string tag, int? planId, int value) {
			Universe.Using(d => {
				d.Feelings.Add(new Feeling {
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

		Plan[] Me.I.QueryPlans(DateTime? start, DateTime? end, bool? done, bool? abandoned) {
			var r = Universe.Using(d => d.Plans.Where(e =>
				e.GodId == Id
				&& (e.AppearTime > (start ?? DateTime.MinValue))
				&& (e.AppearTime < (end ?? DateTime.MaxValue))
				&& (!done.HasValue || e.DoneTime.HasValue)
				&& (!abandoned.HasValue || e.Abandoned == abandoned.Value)).OrderBy(e => e.AppearTime).ToList());
			return r.ToArray();
		}

		public int MyFeelingsCount() {
			return Universe.Using(d => d.Feelings.Count(e => e.GodId == Id));
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
					g.Luck = (double)GetHashCode(name) / int.MaxValue;
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

		public string WakeUp(string dailyContent) {
			return Universe.Using(d => {
				var god = d.Gods.Find(Id);
				var s = TodayState(d);
				if (dailyContent == null) {
					return s?.Content;
				}
				if (s == null) {
					var v = GetHashCode(dailyContent);
					v = Math.Max(god.MinDailyEnergy, v);
					d.DailyStates.Add(s = new DailyState {
						Content = dailyContent,
						GodId = Id
					});
				}
				return dailyContent;
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

		Effort[] Me.I.QueryEfforts(int planId) {
			return Universe.Using(d => d.Plans.Include(e => e.Efforts).FirstOrDefault(e => e.GodId == Id && e.Id == planId).Efforts.ToArray());
		}
	}
}
