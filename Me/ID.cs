using System;

namespace Me {
	public abstract class ID {
		public int Id { get; set; }
		public virtual DateTime AppearTime { get; set; }
		public string Content { get; set; }
	}
}
