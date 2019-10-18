using System;
using System.Collections.Generic;

namespace Quiet {
	public class User : ModelBase {
		public string Name { get; set; }
		public string Phone { get; set; }
		public int Score { get; set; }
		public DateTime LastLogin { get; set; }

		public List<Complain> Complains { get; set; } = new List<Complain>();
	}
}
