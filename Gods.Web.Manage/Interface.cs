using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Gods.Web.Manage {
	public class Interface : IdAndTime {
		public string Description { get; set; }
		public string Name { get; set; }
		public int? CoderId { get; set; }

		public Coder Coder { get; set; }
		public List<ActionRecord> ActionRecords { get; set; } = new List<ActionRecord>();

		[NotMapped]
		public string CurrentCoderName => Coder?.Name;
		[NotMapped]
		public int RecordsCount => ActionRecords?.Count ?? 0;
		[NotMapped]
		public string LastCoderName => ActionRecords.LastOrDefault()?.Coder?.Name;
	}
}