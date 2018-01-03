using System;

namespace BLL.V0 {
	public class Live : Gods.AOP.ModelBase, ILive {
		public void WakeUp() {
			Console.WriteLine(nameof(WakeUp));
		}
	}
}
