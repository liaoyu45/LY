using System.Collections.Generic;

namespace Quiet {
	public interface UserEntrance : Quiet.I {
		Dictionary<string, string> Authentication { get; set; }
		void Login(string name);
		bool Login(string name, string auth);
	}
}
