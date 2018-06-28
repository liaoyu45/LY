using System;
using System.Collections.Generic;

namespace Me {
	public class Plan : ContentBase {
		public int Required { get; set; }
		public string Tag { get; set; }
		public int GodId { get; set; }
		public int Current { get; set; }
		public bool Abandoned { get; set; }
		public DateTime? Finished { get; set; }

		public God God { get; set; }
		public List<Effort> Efforts { get; set; } = new List<Effort>();

		public bool Done => Current > Required;
		public double Difficulty => Math.Round(Required / (double)int.MaxValue, 2);
		public double Percent => Math.Min(1, Math.Round(Current / (double)Required, 2));
		public int Left => Required - Current;
	}
}