using System;

namespace Quiet.Models {
	public class Complain : ModelBase {
		public int UserId { get; set; }
		public int SpamId { get; set; }
		public string Description { get; set; }
		public string Files { get; set; }
		public DateTime ComplainTime { get; set; }
		public ComplainType ComplainType { get; set; }

		public Spam Spam { get; set; }
		public User User { get; set; }
	}
}
