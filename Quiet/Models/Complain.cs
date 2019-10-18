namespace Quiet {
	public class Complain : Abstracts.ModelBase {
		public int UserId { get; set; }
		public int SpamId { get; set; }
		public string Description { get; set; }
		public string Files { get; set; }

		public Spam Spam { get; set; }
		public User User { get; set; }

		public string NNN => Spam?.Name;
	}
}
