using System;
using System.Collections.Generic;

namespace Me {
	public class Plan : ContentBase {
		public int Required { get; set; }
		public string Tag { get; set; }
		public int GodId { get; set; }
		public int Value { get; set; }
		public bool Abandoned { get; set; }
		public DateTime? Finished { get; set; }

		public God God { get; set; }
		public List<Effort> Efforts { get; set; } = new List<Effort>();

		public bool Done => Value > Required;
	}
}