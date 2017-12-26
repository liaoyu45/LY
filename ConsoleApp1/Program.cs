using Gods.AOP;
using System.Reflection;

namespace ConsoleApp1 {
	class EE : Gods.AOP.IValidator {
		public Modes Mode => Modes.Foreigner;

		public bool Inheritable => true;

		public bool Validate(ModelBase obj, MethodBase calling, MethodBase caller) {
			return true;
		}
	}
	class Program {
		static void Main(string[] args) {
			var v = new EE();
			new DailyWork().OpenComputer();
		}
	}
}
