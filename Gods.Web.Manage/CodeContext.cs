using System.Data.Entity;

namespace Gods.Web.Manage {
	class CodeContext : DbContext {
		public static string NameOrConnectionString = nameof(CodeContext);
		static CodeContext() {
			Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CodeContext>());
			using (var d = new CodeContext()) {
				try {
					d.Database.CreateIfNotExists();
				} catch (System.Exception) {

					throw;
				}
			}
		}

		public CodeContext() : base(NameOrConnectionString) { }

		public DbSet<Coder> Coders { get; set; }
		public DbSet<Interface> Interfaces { get; set; }
		public DbSet<ActionRecord> ActionRecords { get; set; }
	}
}
