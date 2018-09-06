using System;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace Gods.Web.Manage {
	public partial class Him : Page, IRouteHandler, System.Web.SessionState.IRequiresSessionState {
		public const string ASPX = nameof(Him) + ".aspx";

		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) => this;
		static bool ever;
		protected override void OnLoad(EventArgs e) {
			if (!ever) {
				ever = true;
				File.WriteAllText(Request.MapPath(ASPX), Properties.Resources.Him.Split(new[] { "CodeBehind" }, StringSplitOptions.RemoveEmptyEntries)[0] + "%>");
				Response.Redirect(ASPX);
				throw null;
			}
			if (IsPostBack) {
				return;
			}
			Web.Him.AllTypeCache.ForEach(ee => {
				new FileInfo(ee.Declare.Assembly.CodeBase);
				new FileInfo(ee.Implement.Assembly.CodeBase);
			});
			eee.DataSource = Web.Him.AllTypeCache;
		}
		class MyClass {

		}
	}
}