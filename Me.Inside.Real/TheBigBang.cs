﻿using System.Data.Entity;

namespace Me.Inside.Real {
	class TheBigBang : DropCreateDatabaseIfModelChanges<Universe> {
		protected override void Seed(Universe context) {
			context.Gods.Add(new God { Name = "eee", Password = "111" });
			context.SaveChanges();
			base.Seed(context);
		}
	}
}