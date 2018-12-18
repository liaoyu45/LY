using System;

namespace Me.Inside {
	public interface Soul : I {
		int AllCount();
		int IngCount();
		int NewPlan(string thing);
		Plan LastPlan();
		Plan LastEffort();
		Plan ForgottenPlan();
		Plan[] FromTo(DateTime @from, DateTime to, int skip, int take, int? tagId);
		Tag[] Tags();
		int UpdateTag(int id, string name, byte[] image);
		void DeleteTag(int id);
	}
}