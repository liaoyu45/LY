using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Me {
	[Export(typeof(Body))]
	public class OQWIERU : Soul, Body {
		public int Happiness { get; set; }

		public List<State> States { get; } = new List<State>();

		public int Desire(Thing t) {
			return 1;
		}

		public int Feel(Thing t) {
			return 1;
		}

		public int Get(Thing t) {
			return 1;
		}

		public int Pay(You y, Thing some) {
			return 1;
		}
	}
}
