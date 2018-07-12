using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Me {
	public class Plan : ContentBase {
		[MaxLength(8)]
		public string Tag { get; set; }
		public int GodId { get; set; }

		public God God { get; set; }
		public List<Effort> Efforts { get; set; } = new List<Effort>();
	}
}