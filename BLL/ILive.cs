
using System.Collections.Generic;

namespace BLL {
	public interface ILive : I {
		int Left { get; }
		void WakeUp(int time);
		IEnumerable<int> Test();
	}
}