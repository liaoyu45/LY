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
		int Me.I.Pay(int planId, string content) {
			return Universe.Using(d => {
				Effort ee;
				d.Plans.First(e => e.GodId == Id && e.Id == planId).Efforts.Add(ee = new Effort { Content = content });
				d.SaveChanges();
				return ee.Id;
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

		Plan[] Me.I.QueryPlans(DateTime? start, DateTime? end, int skip, int take) {
			Range.Arrange(ref start, ref end);
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

		void Me.I.GiveUp(int planId) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId);
				if (p != null) {
					d.Plans.Remove(p);
				}
			});
		}

		void Me.I.Finish(int planId) {
			Universe.Using(d => {
				var plan = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId) ?? new Plan();
				plan.Done = true;
			});
		}

		Effort[] Me.I.QueryEfforts(int planId, DateTime? start, DateTime? end) {
			Range.Arrange(ref start, ref end);
			return Universe.Using(d => d.Plans.Include(e => e.Efforts).FirstOrDefault(e => e.GodId == Id && e.Id == planId).Efforts.Where(e => (!start.HasValue || e.AppearTime > start) && (!end.HasValue || e.AppearTime < end)).ToArray());
		}
	}
	class Range {
		public static void Arrange<T>(ref T? small, ref T? big) where T : struct, IComparable<T> {
			if (small.HasValue && big.HasValue && big.Value.CompareTo(small.Value) < 0) {
				big = null;
			}
		}
	}
}
