using Quiet.Models;

namespace Quiet {
	public interface IUserMain : Quiet.I {
		User Info { get; set; }
		int Report(string name, string websit, string phone, string data, string description);
		Spam[] RecentlySpams(int page);
		Complain[] MyComplains();
		int Fight(string data);
		bool Login(string name, string password);
	}
}
