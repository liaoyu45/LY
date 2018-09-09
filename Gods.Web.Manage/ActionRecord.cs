﻿using System;

namespace Gods.Web.Manage {
	public class ActionRecord : IdAndTime {
		public int InterfaceId { get; set; }
		public int CoderId { get; set; }
		public string Content { get; set; }

		public Interface Interface { get; set; }
		public Coder Coder { get; set; }
	}

	public abstract class IdAndTime {
		public int Id { get; set; }
		public virtual DateTime AppearTime { get; set; } = DateTime.Now;
	}
}