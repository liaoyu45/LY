using System.Data.Entity;

namespace Me.World {
	public class Db : DbContext {
		static Db() {
			Database.SetInitializer(new DropCreateDatabaseIfModelChanges<Db>());
		}

		public static Db Instance { get; set; } = new Db();

		public DbSet<Effort> Efforts { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<Mine> Mines { get; set; }
		public DbSet<DailyState> DailyStates { get; set; }
		public DbSet<God> Gods{ get; set; }
	}
}
