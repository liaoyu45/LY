using System;

namespace Me.Inside {
	public interface Soul : Me.I {
		int Id { get; set; }
		string WakeUp(string name, string password);
		void Sleep();
		int Pay(int planId, string content, bool done);
		int Desire(string thing);
		void GiveUp(int planId);
		Plan[] QueryPlans(DateTime? start, bool? done);
		Effort[] QueryEfforts(int planId);
		void DeleteEffort(int id);
	}
}