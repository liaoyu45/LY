using Me.Inside;
using System;
using System.Linq;

namespace Me.Real {
	public partial class Soul : System.Web.UI.Page, Inside.Soul {
		public int Id { get; set; }

		int Me.Inside.Soul.NewPlan(string thing) {
			return Universe.Using(d => {
				var plan = new Plan {
					Content = thing
				};
				d.Plans.Add(plan);
				d.SaveChanges();
				return plan.Id;
			});
		}

		Plan Inside.Soul.ForgottenPlan() {
			throw new NotImplementedException();
		}

		Plan Inside.Soul.LastEffort() {
			throw new NotImplementedException();
		}

		Plan Inside.Soul.LastPlan() {
			throw new NotImplementedException();
		}

		int Inside.Soul.AllCount() {
			return Universe.Using(e => e.Plans.Count());
		}

		int Inside.Soul.IngCount() {
			return Universe.Using(e => e.Plans.Count(a => a.DoneTime.HasValue));
		}

		Plan[] Inside.Soul.FromTo(DateTime @from, DateTime to, int skip, int take, string tag) {
			return Universe.Using(u => {
				var r = from p in u.Plans
						where p.AppearTime > @from && p.AppearTime < to && (tag == null || p.Tag == tag)
						orderby p.AppearTime descending
						select p;
				if (take == 0) {
					throw new Exception(r.Count().ToString());
				}
				return r.Skip(skip).Take(take).ToArray();
			});
		}
	}
}