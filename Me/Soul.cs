using System;

namespace Me {
	public interface Soul : I {
		int MyAverageFeeling();
		int Pay(int planId, int some);
		int Desire(string thing);
		int Feel(string content, string tag, double value, long last, int? planId);
		int GiveUp(int planId);
		Plan[] Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int?pMax, bool done);
	}
}
