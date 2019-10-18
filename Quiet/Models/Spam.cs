using System.Collections.Generic;

namespace Quiet.Models {
	public class Spam : ModelBase {
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Website { get; set; }
		public string Email { get; set; }
		public string MoreData { get; set; }
		public string Description { get; set; }

		public List<Complain> Complains { get; set; } = new List<Complain>();
	}
}
