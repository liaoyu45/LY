using System.Collections.Generic;

namespace Quiet.Models {
	public class User : ModelBase {
		public string Phone { get; set; }
		public string Password { get; set; }
		public int Score { get; set; }

		public List<Complain> Complains { get; set; } = new List<Complain>();
	}
}
