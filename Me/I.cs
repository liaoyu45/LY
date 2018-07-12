using System;

namespace Me {
	public interface I : World.I {
		int Id { get; set; }
		string FindMyself(string name, string password);
		void Leave();
		void Pay(int planId, string content);
		void Desire(string thing);
		void GiveUp(int planId);
		Plan[] QueryPlans(DateTime? start, DateTime? end, string tag);
		Effort[] QueryEfforts(int planId, DateTime? start, DateTime? end);
	}
}