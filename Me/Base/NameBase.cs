using System.ComponentModel.DataAnnotations;

namespace Me {
	public abstract class NameBase : IdAndTime {
		[Required]
		public string Name { get; set; }
	}
}
