namespace Me {
	public class Mine : ID {
		public double Value { get; set; }
		public long Last { get; set; }
		public int FeelingId { get; set; }

		public Feeling Feeling { get; set; }
	}
}
