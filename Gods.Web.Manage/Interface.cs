using System.Collections.Generic;

namespace Gods.Web.Manage {
	public class Interface : IdAndTime {
		public string Description { get; set; }
		public string Name { get; set; }
		public int? CoderId { get; set; }

		public Coder Coder { get; set; }
		public List<ActionRecord> ActionRecords { get; set; } = new List<ActionRecord>();
	}
}