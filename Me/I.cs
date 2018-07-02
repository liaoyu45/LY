using System;

namespace Me {
	public interface I : World.I {
		int Id { get; set; }
		string FindMyself(string name, string password);
		DailyState WakeUp(string dailyContent);
		void Leave();
		int MyFeelingsCount();
		PayData Pay(int planId, string content);
		DesireData Desire(string thing, bool test);
		void Feel(string content, string tag, int? planId, int value);
		void GiveUp(int planId, bool forever);
		void Resume(int planId);
		QueryData ArrangePrepare(DateTime? start, DateTime? end, int? minRequired, int? maxRequired, int? minValue, int?maxValue, bool? done, bool? abandoned);
		Plan[] ArrangeQuery(int query, int start, int end);
	}
}