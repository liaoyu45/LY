using System.Data.Entity;

namespace Me.World {
	class DbSeed : DropCreateDatabaseIfModelChanges<Db> {
		protected override void Seed(Db context) {
			context.Gods.Add(new God { Name = "MeMySelfAndI" });
			context.SaveChanges();
			base.Seed(context);
		}
	}
}
