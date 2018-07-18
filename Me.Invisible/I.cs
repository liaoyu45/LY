using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Me.Invisible {
	public class I : Me.I {
		public int Id { get; set; }
		public Dictionary<int, Expression<Func<Plan, bool>>> Query { get; set; } = new Dictionary<int, Expression<Func<Plan, bool>>>();

		private DateTime today = DateTime.Now.Date;
		private DateTime tomorrow = DateTime.Now.AddDays(1).Date;
		static private DateTime mine = DateTime.Parse("1990-03-13");
		void Me.I.Pay(int planId, string content) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId);
				p?.Efforts.Add(new Effort { Content = content });
			});
		}

		int Me.I.Desire(string thing) {
			return Universe.Using(d => {
				var plan = new Plan {
					GodId = Id,
					Content = thing
				};
				d.Plans.Add(plan);
				d.SaveChanges();
				return plan.Id;
			});
		}

		void Me.I.GiveUp(int planId) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId);
				if (p == null) {
					return;
				}
				d.Plans.Remove(p);
			});
		}

		Plan[] Me.I.QueryPlans(DateTime? start, DateTime? end, int skip, int take) {
			Rang.Arrange(ref start, ref end, () => start.Value.AddDays(1));
			var r = Universe.Using(d => {
				d.Database.Log = e => Console.WriteLine(e);
				return d.Plans.Where(e =>
e.GodId == Id
&& (e.AppearTime > (start ?? mine))
&& (e.AppearTime < (end ?? DateTime.Now))
).OrderByDescending(e => e.AppearTime).Skip(skip).Take(take).ToList();
			});
			return r.ToArray();
		}

		string Me.I.WakeUp(string name, string password) {
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

		void Me.I.Sleep() {
			Id = 0;
		}

		Effort[] Me.I.QueryEfforts(int planId, DateTime? start, DateTime? end) {
			Rang.Arrange(ref start, ref end, () => start.Value.AddDays(1));
			return Universe.Using(d => d.Plans.Include(e => e.Efforts).FirstOrDefault(e => e.GodId == Id && e.Id == planId).Efforts.Where(e => (!start.HasValue || e.AppearTime > start) && (!end.HasValue || e.AppearTime < end)).ToArray());
		}
	}
	class Rang {
		public static void Arrange<T>(ref T? small, ref T? big, Func<T> ifNot) where T : struct, IComparable<T> {
			if (small.HasValue && big.HasValue && big.Value.CompareTo(small.Value) < 0) {
				big = ifNot();
			}
		}
	}
}
