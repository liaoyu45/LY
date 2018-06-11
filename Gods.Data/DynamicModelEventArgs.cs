using System;

namespace Gods.Data {
	public class DynamicModelEventArgs : EventArgs {
		public bool Canceled { get; set; }
		public object Model { get; set; }
	}
}
