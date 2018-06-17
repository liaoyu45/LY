using System;

namespace Me {
	public abstract class IdAndTime {
		public int Id { get; set; }
		public virtual DateTime AppearTime { get; set; } = DateTime.Now;
	}
}
