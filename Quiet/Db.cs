using System.Data.Entity;

namespace Quiet {
	class Db : DbContext {
		public Db() : base(nameof(Db)) { }
		public DbSet<Models.User> Users { get; set; }
		public DbSet<Models.Complain> Complains { get; set; }
		public DbSet<Models.Spam> Spams { get; set; }
	}
}
