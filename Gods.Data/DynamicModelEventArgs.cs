using System;

namespace LivingDB {
	public class DynamicModelEventArgs : EventArgs {
		public bool Canceled { get; set; }
		public object Model { get; set; }
	}
}
