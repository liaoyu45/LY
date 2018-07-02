using System;

namespace Me {
	public class PayData {
		public int Payed { get; set; }
		public double Percent { get; set; }
		public bool Done => Percent >= 1;
	}
}
