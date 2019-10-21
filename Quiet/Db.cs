using System;
using System.Data.Entity;

namespace Quiet {
	public class Db : DbContext {
		public Db() : base(nameof(Db)) { }
		public DbSet<Models.User> Users { get; set; }
		public DbSet<Models.Complain> Complains { get; set; }
		public DbSet<Models.Spam> Spams { get; set; }

		public static void UseDb(Action<Db> use) {
			using (var d = new Db()) {
				use(d);
			}
		}

		public static T UseDb<T>(Func<Db, T> use) {
			using (var d = new Db()) {
				return use(d);
			}
		}
	}
}
