﻿namespace Me.Limit {
	public class LimitIDo : Limit<I> {
		public int Id { get; set; }

		public string GiveUp(int planId) {
			return "请不要放弃";
		}
	}
}