using Me.Inside;
using System;
using System.Data.Entity;
using System.Linq;

namespace Me.Real {
	public class Universe : DbContext {
		public DbSet<Effort> Efforts { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<Tag> Tags { get; set; }

		static Universe() {
			Database.SetInitializer(new TheBigBang());
		}

		public Universe() : base(nameof(Me)) { }

		public static T Using<T>(Func<Universe, T> func) {
			using (var d = new Universe()) {
				var r = func(d);
				if (d.ChangeTracker.HasChanges()) {
					d.SaveChanges();
				}
				return r;
			}
		}

		public static void Using(Action<Universe> action) {
			Using(d => {
				action(d);
				var c = d.ChangeTracker.Entries().Count();
				if (c > 0) {
					d.SaveChanges();
				}
				return c;
			});
		}
	}
}
