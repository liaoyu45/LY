namespace Me.Inside {
	public interface Desire : Me.I {
		int Pay(int planId, string content, bool done);
		void GiveUp(int planId);
		void DeleteEffort(int id);
	}
}
