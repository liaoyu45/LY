using Me.Inside;
using System.Data.Entity;

namespace Me.Real {
	class TheBigBang : DropCreateDatabaseIfModelChanges<Universe> {
		protected override void Seed(Universe context) {
			context.SaveChanges();
			base.Seed(context);
		}
	}
}
