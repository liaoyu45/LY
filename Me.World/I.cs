using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;

namespace Me.World {
	[Export(typeof(Soul))]
	public class I : DbContext, Soul, Me.I {
		public DbSet<Effort> Efforts { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<Mine> Mines { get; set; }
		public DbSet<Feeling> Feelings { get; set; }
		public DbSet<DailyState> DailyStates { get; set; }
		private static Random r = new Random();

		private static int avgFeeling = -1;
		public I() {
			if (avgFeeling == -1) {
				Database.SetInitializer(new DropCreateDatabaseIfModelChanges<I>());
				ResetAvgFeeling();
			}
		}

		private void ResetAvgFeeling() {
			if (Mines.Any()) {
				avgFeeling = (int)Mines.Average(e => e.Value);
			}
		}

		int Soul.Pay(int planId, int some) {
			var plan = Plans.Find(planId);
			if (plan.Done) {
				return 0;
			}
			Effort e;
			plan.Efforts.Add(e = new Effort { Payed = some });
			SaveChanges();
			e.Real = r.Next(some);
			for (int i = 0; i < avgFeeling; i++) {
				e.Real = r.Next(some);
			}
			SaveChanges();
			return e.Real;
		}

		int Soul.Desire(string thing) {
			var n = DateTime.Now;
			var p = Plans.Add(new Plan {
				Content = thing,
				AppearTime = n,
				Required = r.Next(thing.GetHashCode() / 2 + n.GetHashCode() / 2, int.MaxValue),
			});
			SaveChanges();
			return p.Required;
		}

		int Soul.Feel(string t) {
			if (t == null) {
				return avgFeeling;
			}
			return 1;
		}

		int Soul.GiveUp(int planId) {
			throw new NotImplementedException();
		}

		Plan[] Soul.Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int? pMax, bool done) {
			return null;
		}
	}
}
