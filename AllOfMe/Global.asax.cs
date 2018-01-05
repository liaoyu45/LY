using System;

namespace AllOfMe {
	public class Global : System.Web.HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			new BLL.V0.Work().GoWork();
			return;
			Gods.Web.Javascript.WriteLikeSCharp();
		}
	}
}