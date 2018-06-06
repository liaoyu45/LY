using System.Collections.Generic;

namespace Me {
	public class Feeling : ID {
		public string Name { get; set; }

		public List<Mine> Mines { get; set; }
	}
}
