using Me.Inside;
using System.Data.Entity;

namespace Me.Real {
	class TheBigBang : DropCreateDatabaseIfModelChanges<Universe> {
		protected override void Seed(Universe context) {
			context.Tags.Add(new Tag { Name = "衣" });
			context.Tags.Add(new Tag { Name = "食" });
			context.Tags.Add(new Tag { Name = "住" });
			context.Tags.Add(new Tag { Name = "行" });
			context.Tags.Add(new Tag { Name = "乐" });
			context.SaveChanges();
			base.Seed(context);
		}
	}
}
