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

		PayData Me.I.Pay(int planId, string content) {
			return Universe.Using(d => {
				var p = d.Plans.Find(planId);
				if (p.Done) {
					throw new Exception(p.Content + "-已完成");
				}
				var g = d.Gods.Find(Id);
				var payed = (int)(GetHashCode(content) / (double)int.MaxValue * p.Required * g.Luck / p.MinEffortsCount);
				var s = TodayState(d);
				if (s.Energy < payed) {
					throw new Exception("今日能量不足");
				}
				s.Energy -= payed;
				p.Efforts.Add(new Effort { Content = content, Payed = payed });
				p.Current += payed;
				if (p.Done) {
					p.Current = p.Required;
					p.DoneTime = DateTime.Now;
					g.Luck += new Random().NextDouble() * (1 - g.Luck);
				}
				return new PayData {
					Payed = payed,
					Percent = p.Percent
				};
			});
		}

		DesireData Me.I.Desire(string thing, bool test) {
			return Universe.Using(d => {
				var s = TodayState(d);
				if (s.DesireCount < 1) {
					throw new Exception($"当日尝试的次数用尽：{DateTime.Now.ToString("yyyy-MM-dd")}/，已用 {s.EarlierDesireCount} 次。");
				}
				s.DesireCount--;
				var r = GetHashCode(thing);
				var data = new DesireData {
					CountLeft = s.DesireCount,
					Required = r
				};
				if (test) {
					return data;
				}
				d.Plans.Add(new Plan {
					GodId = Id,
					Content = thing,
					Required = (int)Math.Min(r / d.Gods.Find(Id).Luck, int.MaxValue),
				});
				return data;

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
				&& (!done.HasValue || e.DoneTime.HasValue)
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

		public DailyState WakeUp(string dailyContent) {
			return Universe.Using(d => {
				var god = d.Gods.Find(Id);
				var s = TodayState(d);
				if (dailyContent == null) {
					return s;
				}
				if (s == null) {
					var v = GetHashCode(dailyContent);
					var t = Math.Log(dailyContent.Length);
					var r = new Random();
					for (var i = 0; i < t; i++) {
						v = r.Next(v, int.MaxValue);
					}
					d.DailyStates.Add(s = new DailyState(v) {
						Content = dailyContent,
						GodId = Id
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
