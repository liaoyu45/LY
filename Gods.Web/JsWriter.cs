using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Gods.Web {
	internal class JsWriter : IRouteHandler, IHttpHandler {
		private JObject json;

		public JsWriter(JObject json) {
			this.json = json;
		}

		bool IHttpHandler.IsReusable => false;

		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) => this;

		void IHttpHandler.ProcessRequest(HttpContext context) {
			context.Response.ContentType = "application/json";
			context.Response.Write($@"
window.god = window.god || (window.god = {{}});
(god.CSharp || (god.CSharp = {{}})).Me = ".Trim() + json[context.Request.RawUrl.Split('?')[1]]);
		}
	}
}