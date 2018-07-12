namespace Me.Limit {
	public class LimitIDo : World.ILimit<I> {
		public int Id { get; set; }

		public string GiveUp(int planId) {
			return "Please, do not give up.";
		}
	}
}
