using System;

namespace Me {
	public interface Soul : I {
		int Pay(int planId, int some);
		int Desire(string thing);
		int Feel(string t);
		int GiveUp(int planId);
		Plan[] Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int?pMax, bool done);
	}
}
