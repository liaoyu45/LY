using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace BLL.V0 {
	[Export(typeof(ILive))]
	public class Live : Gods.AOP.Model, ILive {
		public int Left {
			get {
				return 33;
			}
		}

		public IEnumerable<int> Test() {
			yield return 1;
		}

		public void WakeUp(int time) {
			Console.WriteLine(nameof(WakeUp));
		}
	}
}
