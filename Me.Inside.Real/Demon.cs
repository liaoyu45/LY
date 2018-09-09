using System;
using System.Data.Entity;
using System.Linq;

namespace Me.Inside.Real {
	public class Demon : Soul {
		public int Id { get; set; }

		private DateTime today = DateTime.Now.Date;
		private DateTime tomorrow = DateTime.Now.AddDays(1).Date;
		static private DateTime mine = DateTime.Parse("1990-03-13");
		int Soul.Pay(int planId, string content, bool done) {
			return Universe.Using(d => {
				Effort ee;
				var p = d.Plans.First(e => e.GodId == Id && e.Id == planId);
				if (p.Done) {
					throw new Exception("已完成");
				}
				p.Efforts.Add(ee = new Effort { Content = content });
				p.Done = done;
				d.SaveChanges();
				return ee.Id;
			});
		}

		int Soul.Desire(string thing) {
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

		Plan[] Soul.QueryPlans(DateTime? start, bool? done) {
			var r = Universe.Using(d => d.Plans.Where(e => e.GodId == Id && (start == null || e.AppearTime > start) && (done == null || e.Done == done.Value)).OrderByDescending(e => e.AppearTime).ToList());
			return r.ToArray();
		}

		string Soul.WakeUp(string name, string password) {
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

		void Soul.Sleep() {
			Id = 0;
		}

		void Soul.GiveUp(int planId) {
			Universe.Using(d => {
				var p = d.Plans.FirstOrDefault(e => e.GodId == Id && e.Id == planId);
				if (p != null) {
					d.Plans.Remove(p);
				}
			});
		}

		Effort[] Soul.QueryEfforts(int planId) {
			return Universe.Using(d => d.Efforts.Include(a => a.Plan).Where(e => e.PlanId == planId && e.Plan.GodId == Id).ToArray());
		}

		void Soul.DeleteEffort(int id) {
			Universe.Using(d => {
				var e = d.Efforts.Include(a => a.Plan).FirstOrDefault(a => a.PlanId == id && a.Plan.GodId == Id);
				if (e != null) {
					d.Efforts.Remove(e);
				}
			});
		}
	}
}