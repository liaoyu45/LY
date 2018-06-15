using System;
using System.Data.Entity;
using System.Linq;

namespace Me.Invisible {
	public class Db : DbContext {
		static Db() {
			Database.SetInitializer(new DbSeed());
		}

		public static Db Instance { get; set; } = new Db();

		public static T Using<T>(Func<Db, T> func) {
			using (var d = new Db()) {
				var r = func(d);
				if (d.ChangeTracker.HasChanges()) {
					d.SaveChanges();
				}
				return r;
			}
		}

		public static void Using(Action<Db> action) {
			Using(d => {
				action(d);
				return d.ChangeTracker.Entries().Count();
			});
		}

		public DbSet<Effort> Efforts { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<Mine> Mines { get; set; }
		public DbSet<DailyState> DailyStates { get; set; }
		public DbSet<God> Gods { get; set; }
	}
}
