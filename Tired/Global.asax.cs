using System;

namespace Tired {
	public class Global : System.Web.HttpApplication {
		protected void Application_Start(object sender, EventArgs e) {
			Gods.Web.Him.Create<Quiet.LimitI<Quiet.I>>();
			using var eeq = new System.Threading.AutoResetEvent(true);
		}
	}
}