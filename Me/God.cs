using System.Collections.Generic;

namespace Me {
	public class God : NameBase {
		public string Password { get; set; }

		public List<Plan> Plans { get; set; } = new List<Plan>();
	}
}
