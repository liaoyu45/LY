using System.Data.Entity;

namespace Me {
	class TheBigBang : DropCreateDatabaseIfModelChanges<Universe> {
		protected override void Seed(Universe context) {
			context.Gods.Add(new God { Name = "MeMySelfAndI", Password = "123" });
			context.SaveChanges();
			base.Seed(context);
		}
	}
}
