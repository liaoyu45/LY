using System.Data.Entity;

namespace Me.Invisible {
	class TheBigBang : DropCreateDatabaseAlways<Universe> {
		protected override void Seed(Universe context) {
			context.Gods.Add(new God { Name = "eee", Password = "111" });
			context.SaveChanges();
			base.Seed(context);
		}
	}
}
