﻿using System;
using System.Collections.Generic;

namespace Me.Inside {
	public class Plan : ContentBase {
		public DateTime? DoneTime { get; set; }
		public int? TagId { get; set; }

		public Tag Tag { get; set; }
		public double Progress { get; set; }
		public List<Effort> Efforts { get; set; } = new List<Effort>();
	}
}