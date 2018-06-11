namespace Me {
	public class Mine : ContentBase {
		public double Value { get; set; }
		public long Last { get; set; }
		public string Tag { get; set; }
		public int? PlanId { get; set; }

		public Plan Plan { get; set; }
	}
}
