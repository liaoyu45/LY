using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1 {
	[Gods.AOP.Validator(typeof(EE))]
	class DailyWork : Gods.AOP.ModelBase {
		internal void OpenComputer() {
			Console.WriteLine();
		}
	}
}
