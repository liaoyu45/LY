using System;

namespace Me.Inside {
	public interface Soul : Me.I {
		int Id { get; set; }
		string WakeUp(string name, string password);
		void Sleep();
		int Pay(int planId, string content);
		int Desire(string thing);
		void GiveUp(int planId);
		void Finish(int planId);
		Plan[] QueryPlans(DateTime? start, DateTime? end, int skip, int take);
		Effort[] QueryEfforts(int planId, DateTime? start, DateTime? end, int skip, int take);
	}
}