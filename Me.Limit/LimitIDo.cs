namespace Me.Limit {
	public class LimitIDo : World.ILimit<I> {
		public int Id { get; set; }

		public string GiveUp(int planId, bool forever) {
			if (forever) {
				return "Please, do not give up."; 
			}
			return null;
		}
	}
}
