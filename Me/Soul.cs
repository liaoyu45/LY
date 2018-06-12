using System;

namespace Me {
	public interface Soul : I {
		int MyAverageFeeling();
		int Pay(int planId, int some);
		int Desire(string thing);
		int Feel(string content, string tag, double value, long last, int? planId);
		int GiveUp(int planId);
		void Suicide();
		Plan[] Arrange(DateTime? start, DateTime? end, int? min, int? max, int? pMin, int?pMax, bool done);
	}
}
namespace Me.TestNamespace {
	public interface TestI :I {
		int GGGGG(int i);
	}
	[System.ComponentModel.Composition.Export(typeof(TestI))]
	public class WQER : TestI {
		int TestI.GGGGG(int i) {
			Console.WriteLine(i);
			return i;
		}
	}
	[System.ComponentModel.Composition.Export(typeof(Limit<TestI>))]
	public class WERER : Limit<TestI> {
		public int GGGGG(int i) {
			return 123123121;
		}
	}
}