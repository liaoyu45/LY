using System;

namespace Me {
	public interface I : World.I {
		int Id { get; set; }
		string FindMyself(string name, string password);
		string WakeUp(string dailyContent);
		void Leave();
		int MyFeelingsCount();
		void Pay(int planId, string content);
		void Desire(string thing);
		void Feel(string content, string tag, int? planId, int value);
		void GiveUp(int planId, bool forever);
		void Resume(int planId);
		Plan[] QueryPlans(DateTime? start, DateTime? end, bool? done, bool? abandoned);
		Effort[] QueryEfforts(int planId);
	}
}