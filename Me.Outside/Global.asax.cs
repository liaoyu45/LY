using System;

namespace Me.Outside {
	public class Global : System.Web.HttpApplication {
		protected void Application_Start(object sender, EventArgs e) {
			Gods.Web.Him.Create<Limit<I>>();
			Gods.Web.Manage.Him.Create();
		}
		protected void Session_Start(object sender, EventArgs e) { }

		protected void Session_End(object sender, EventArgs e) { }
	}
}