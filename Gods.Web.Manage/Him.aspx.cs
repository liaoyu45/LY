using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
				File.WriteAllText(Request.MapPath(ASPX), Properties.Resources.Him.Replace("CodeBehind=\"~/ Him.aspx.cs\" ", string.Empty));
				Response.Redirect(ASPX);
				throw null;
			}
			if (IsPostBack) {
				return;
			}
			var c = new CodeContext();
			foreach (var item in Web.Him.AllTypeCache) {
				if (c.Interfaces.Any(i => i.Name == item.Declare.FullName)) {
					continue;
				}
				c.Interfaces.Add(new Interface {
					Name = item.Declare.FullName,
					Description = item.Declare.GetCustomAttribute<DescriptionAttribute>()?.Description,
					ActionRecords = new List<ActionRecord>(item.GetImplement() == null ? Enumerable.Empty<ActionRecord>() : new[] {new ActionRecord {
						Content = item.Implement.GetCustomAttribute<DescriptionAttribute>()?.Description
					}})
				});
				c.SaveChanges();
			}
			eee.DataSource = c.Interfaces.Include(a => a.ActionRecords).Include(a => a.Coder).ToList();
			eee.DataBind();
			c.Dispose();
		}

		public static void Create() {
			RouteTable.Routes.Add(new Route(ASPX, new Him()));
		}
	}
}