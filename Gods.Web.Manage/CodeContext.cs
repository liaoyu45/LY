using System.Data.Entity;

namespace Gods.Web.Manage {
	class CodeContext : DbContext {
		public DbSet<Coder> Coders { get; set; }
		public DbSet<Interface> Interfaces { get; set; }
		public DbSet<ActionRecord> ActionRecords { get; set; }
	}
}
