using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace BLL.V0 {
	[Export(typeof(ILive))]
	public class Live : Gods.AOP.Model, ILive {
		private const int V = 1;

		public int Left { get; set; }

		[Gods.Web.Cache(V)]
		public IEnumerable<int> Test() {
			yield return 1;
		}
		[Gods.Web.Cache(-V)]
		public void WakeUp(int time) {
			Console.WriteLine(nameof(WakeUp));
		}
	}
}
