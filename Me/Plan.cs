using System.Collections.Generic;

namespace Me {
	public class Plan : ContentBase {
		public int Required { get; set; }
		public bool Done { get; set; }
		public string Tag { get; set; }

		public List<Effort> Efforts { get; set; } = new List<Effort>();
	}
}