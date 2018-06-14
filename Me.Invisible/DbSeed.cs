using System.Data.Entity;

namespace Me.Invisible {
	class DbSeed : DropCreateDatabaseIfModelChanges<Db> {
		protected override void Seed(Db context) {
			context.Gods.Add(new God { Name = "MeMySelfAndI" });
			context.SaveChanges();
			base.Seed(context);
		}
	}
}
