using System.Collections.Generic;

namespace Quiet {
	public class Spam : Abstracts.ModelBase {
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Website { get; set; }
		public string Description { get; set; }
		public WarnLevel WarnLevel { get; set; }
		public int ComplainsCount { get; set; }

		public List<Complain> Complains { get; set; } = new List<Complain>();
	}
}
