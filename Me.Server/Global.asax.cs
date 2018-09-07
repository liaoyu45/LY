using System;

namespace Me.Server {
	public class Global : System.Web.HttpApplication {
		protected void Application_Start(object sender, EventArgs e) {
			Gods.Web.Him.Create<Limit<I>>();
			Gods.Web.Manage.Him.Create();
		}
	}
}