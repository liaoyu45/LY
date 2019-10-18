using Quiet.Models;

namespace Quiet {
	public interface UserMain : Quiet.I {
		User Info { get; set; }
		int Report(string name, string websit, string phone, string data, string description);
		Spam[] RecentlySpams();
		int Fight(string data);
	}
}
