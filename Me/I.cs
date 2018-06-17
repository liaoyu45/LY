using System;

namespace Me {
	public interface I : World.I {
		int Id { get; set; }
		int FindMyself(string name);
		int Awake(string name, string dailyContent);
		int MyAverageFeeling();
		int Pay(int planId, int some);
		int Desire(string thing);
		int Feel(string content, string tag, double value, DateTime? appearTime, int? planId);
		int GiveUp(int planId);
		Plan[] Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int?pMax, bool done);
	}
}