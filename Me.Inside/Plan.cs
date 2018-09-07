using System.Collections.Generic;

namespace Me.Inside {
	public class Plan : ContentBase {
		public int GodId { get; set; }
		public bool Done { get; set; }

		public God God { get; set; }
		public List<Effort> Efforts { get; set; } = new List<Effort>();
	}
}