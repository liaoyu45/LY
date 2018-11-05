using Me.Inside;
using System;
using System.Linq;
using System.Data.Entity;

namespace Me.Real {
	public partial class Desire : System.Web.UI.Page, Me.Inside.Desire {
		public int Id { get; set; }

		void Me.Inside.Desire.GiveUp(int planId) {
			Universe.Using(d => {
				var p = d.Plans.Find(planId);
				if (p != null) {
					d.Plans.Remove(p);
				}
			});
		}

		void Me.Inside.Desire.DeleteEffort(int id) {
			Universe.Using(d => {
				var e = d.Efforts.Include(a => a.Plan).FirstOrDefault(a => a.PlanId == id);
				if (e != null) {
					d.Efforts.Remove(e);
				}
			});
		}

		int Me.Inside.Desire.Pay(int planId, string content, bool done) {
			return Universe.Using(d => {
				Effort ee;
				var p = d.Plans.Find(planId);
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
	}
}