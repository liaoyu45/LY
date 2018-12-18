using Me.Inside;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Me.Real {
	public partial class SoulPage : System.Web.UI.Page, Soul {
		public int Id { get; set; }

		int Soul.NewPlan(string thing) {
			return Universe.Using(d => {
				var plan = new Plan {
					Content = thing
				};
				d.Plans.Add(plan);
				d.SaveChanges();
				return plan.Id;
			});
		}

		Plan Soul.ForgottenPlan() {
			throw new NotImplementedException();
		}

		Plan Soul.LastEffort() {
			throw new NotImplementedException();
		}

		Plan Soul.LastPlan() {
			throw new NotImplementedException();
		}

		int Soul.AllCount() {
			return Universe.Using(e => e.Plans.Count());
		}

		int Soul.IngCount() {
			return Universe.Using(e => e.Plans.Count(a => a.DoneTime.HasValue));
		}

		Plan[] Soul.FromTo(DateTime @from, DateTime to, int skip, int take, int? tagId) {
			return Universe.Using(u => {
				var r = from p in u.Plans
						where p.AppearTime > @from && p.AppearTime < to && (tagId == null || p.TagId == tagId)
						orderby p.AppearTime descending
						select p;
				if (take == 0) {
					throw new Exception(r.Count().ToString());
				}
				return r.Skip(skip).Take(take).ToArray();
			});
		}

		Tag[] Soul.Tags() {
			return Universe.Using(e => e.Tags.ToArray());
		}

		static string path = new System.IO.FileInfo(typeof(SoulPage).Assembly.CodeBase.Replace("file:///", "")).Directory.FullName + "\\images\\";

		int Soul.UpdateTag(int id, string name, byte[] image) {
			var ids = id > 0 ? new object[] { id } : new object[0];
			var t = Universe.AddOrUpdate<Tag>(new Dictionary<string, object> { { nameof(Tag.Name), name } }, ids);
			if (image != null) {
				var f = path + t.Id + ".png";
				new System.IO.FileInfo(f).Directory.Create();
				System.IO.File.WriteAllBytes(f, image);
			}
			return t.Id;
		}
		void Soul.DeleteTag(int id) {
			Universe.Using(e => e.Tags.Remove(e.Tags.Find(id)));
		}
	}
}