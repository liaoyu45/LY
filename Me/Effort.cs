namespace Me {
	public class Effort : ContentBase {
		public int Payed { get; set; }
		public int Real { get; set; }
		public int PlanId { get; set; }

		public Plan Plan { get; set; }
	}
}
