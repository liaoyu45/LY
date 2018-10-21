using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.UI;

namespace Gods.Web.Manage {
	public partial class Him : Page, System.Web.SessionState.IRequiresSessionState {
		protected override void OnLoad(EventArgs e) {
			if (IsPostBack) {
				return;
			}
			interfaces.DataSource = Gods.Web.Him.AllTypeCache.OrderBy(t => t.Declare.FullName);
			interfaces.DataBind();
		}

		public static void Create() {
			File.WriteAllText(HostingEnvironment.MapPath($"/{nameof(Him)}.aspx"), Properties.Resources.Him.Replace("CodeBehind=\"~/ Him.aspx.cs\" ", string.Empty));
		}

	}
}