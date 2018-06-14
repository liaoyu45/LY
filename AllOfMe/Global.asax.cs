using System;

namespace AllOfMe {
	public class Global : System.Web.HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			Gods.Web.Him.Create<World.ILimit<World.I>>(new Gods.Web.His());
		}
	}
}