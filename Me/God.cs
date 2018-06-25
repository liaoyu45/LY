using System.Collections.Generic;

namespace Me {
	public class God : NameBase {
		public string Password { get; set; }
		public List<DailyState> DailyStates { get; set; } = new List<DailyState>();
		public List<Possession> Possessions { get; set; } = new List<Possession>();
		public List<Plan> Plans { get; set; } = new List<Plan>();
	}
}
