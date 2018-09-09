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
			context.Response.ContentType = "application/x-javascript";
			var n = context.Request.RawUrl.Split('?')[1].Split('.')[0];
			context.Response.Write($@"
localStorage.setItem(""MakeJavasciptLookLikeCSharp"", JSON.stringify({{ Key: ""{Him.his.AjaxKey}"", Url: ""{context.Request.Url.ToString().Replace(context.Request.RawUrl, string.Empty)}/{Him.his.AjaxRoute}"" }}))
window.god = window.god || (window.god = {{}});
god.CSharp = {{}};
god.CSharp.{n} = ".TrimStart() + json[n]);
		}
	}
}