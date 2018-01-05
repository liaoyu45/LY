using System;

namespace BLL.V0 {
	public class Live : Gods.AOP.Model, ILive {
		public void WakeUp(int time) {
			Console.WriteLine(nameof(WakeUp));
		}
	}
}
