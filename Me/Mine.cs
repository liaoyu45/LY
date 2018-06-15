namespace Me {
	public class Mine : ContentBase {
		public double Value { get; set; }
		public string Tag { get; set; }
		public int? PlanId { get; set; }
		public int GodId { get; set; }

		public Plan Plan { get; set; }
		public God God { get; set; }
	}
}
