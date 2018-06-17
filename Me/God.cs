﻿using System.Collections.Generic;

namespace Me {
	public class God : NameBase {
		public int Luck { get; set; }
		public List<DailyState> DailyStates { get; set; } = new List<DailyState>();
		public List<Possession> Possessions { get; set; } = new List<Possession>();
	}
}
