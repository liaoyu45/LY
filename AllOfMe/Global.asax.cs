﻿using BLL;
using System;

namespace AllOfMe {
	public class Global : System.Web.HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			Gods.Web.Him.Create<I>();
			Gods.Web.Him.PreferJavascript();
		}
	}
}