using System;
using System.Data.Entity;
using System.Linq;

namespace Me.Inside.Real {
	public class Soul : Inside.Soul {
		public int Id { get; set; }

		private DateTime today = DateTime.Now.Date;
		private DateTime tomorrow = DateTime.Now.AddDays(1).Date;
		static private DateTime mine = DateTime.Parse("1990-03-13");
		int Me.Inside.Soul.Pay(int planId, string content) {
			return Universe.Using(d => {
				Effort ee;
				d.Plans.First(e => e.GodId == Id && e.Id == planId).Efforts.Add(ee = new Effort { Content = content });
				d.SaveChanges();
				return ee.Id;
			});
		}

		int Me.Inside.Soul.Desire(string thing) {
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

		Plan[] Me.Inside.Soul.QueryPlans(DateTime? start, DateTime? end, int skip, int take) {
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

		[System.Obsolete]
		string Me.Inside.Soul.WakeUp(string name, string password) {
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

		void Me.Inside.Soul.Sleep() {
			Id = 0;
		}

		void Me.Inside.Soul.GiveUp(int planId) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId);
				if (p != null) {
					d.Plans.Remove(p);
				}
			});
		}

		void Me.Inside.Soul.Finish(int planId) {
			Universe.Using(d => {
				var plan = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId) ?? new Plan();
				plan.Done = true;
			});
		}

		Effort[] Me.Inside.Soul.QueryEfforts(int planId, DateTime? start, DateTime? end, int skip, int take) {
			Range.Arrange(ref start, ref end);
			return Universe.Using(d => d.Plans.Include(e => e.Efforts).FirstOrDefault(e => e.GodId == Id && e.Id == planId).Efforts.Where(e => e.AppearTime > (start ?? mine) && e.AppearTime < (end ?? DateTime.Now)).Skip(skip).Take(take).ToArray());
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
