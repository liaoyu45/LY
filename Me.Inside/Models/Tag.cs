using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Me.Inside {
	public class Tag {
		public int Id { get; set; }
		[Index(IsUnique = true)]
		[MaxLength(16)]
		public string Name { get; set; }
	}
}
