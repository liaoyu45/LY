namespace Me {
	public class PayData {
		public int Payed { get; set; }
		public double PlanPercent { get; set; }
		public double EffortPercent { get; set; }

		public bool Done => PlanPercent >= 1;
	}
}
