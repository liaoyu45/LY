using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Me {
	public class Plan : ContentBase {
		public string Tag { get; set; }
		public int GodId { get; set; }
		public bool Abandoned { get; set; }
		public DateTime? DoneTime { get; set; }

		public God God { get; set; }
		public List<Effort> Efforts { get; set; } = new List<Effort>();
		[NotMapped]
		public int EffortsCount { get; set; }
	}
}