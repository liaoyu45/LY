using System;

namespace Me {
	public class DailyState : ContentBase {
		public int GodId { get; set; }
		public int Energy { get; set; }
		public int EarlierEnergy { get; set; }
		public int DesireCount { get; set; }
		public int EarlierDesireCount { get; set; }

		public God God { get; set; }

		public DailyState() { }

		public DailyState(int energy) {
			Energy = EarlierEnergy = energy;
			DesireCount = EarlierDesireCount = (int)Math.Log(energy);
		}
	}
}