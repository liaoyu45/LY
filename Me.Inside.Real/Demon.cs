using System;
using System.Data.Entity;
using System.Linq;

namespace Me.Inside.Real {
	public class Demon : Soul {
		public int Id { get; set; }

		int Soul.Pay(int planId, string content, bool done) {
			return Universe.Using(d => {
				Effort ee;
				var p = d.Plans.First(e => e.GodId == Id && e.Id == planId);
				if (p.DoneTime.HasValue) {
					throw new Exception("非法的接口调用，向已完成的计划添加内容。");
				}
				var now = DateTime.Now;
				p.Efforts.Add(ee = new Effort { Content = content, AppearTime = now });
				if (done) {
					p.DoneTime = now;
				}
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

		Plan[] Soul.QueryPlans(int? start, bool? done) {
			if (start == null || start < 1990) {
				start = 1990;
			}
			var s = new DateTime(start.Value, 1, 1);
			return Universe.Using(d => d.Plans.Where(e => e.GodId == Id && e.AppearTime > s && (done == null || e.DoneTime != null == done.Value)).OrderByDescending(e => e.AppearTime).ToArray());
		}

		string Soul.WakeUp(string name, string password) {
			if (Id > 0) {
				return Universe.Using(d => d.Gods.Find(Id)?.Name);
			}
			if (name?.Any() != true || password?.Any() != true) {
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

		void Soul.Sleep() => Id = 0;

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