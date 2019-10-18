namespace Quiet {
	public interface UserMain : I {
		User Info { get; set; }
		Complain[] Report(string name, string websit, string phone);
		void Report(string name, string websit, string phone, WarnLevel level);
		Spam[] DailyQuests();
		int Fight(string data);
	}
}
