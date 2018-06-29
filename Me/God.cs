using System.Collections.Generic;

namespace Me {
	public class God : NameBase {
		public string Password { get; set; }
		public double Luck { get; set; }

		public List<DailyState> DailyStates { get; set; } = new List<DailyState>();
		public List<Feeling> Possessions { get; set; } = new List<Feeling>();
		public List<Plan> Plans { get; set; } = new List<Plan>();
	}
}
