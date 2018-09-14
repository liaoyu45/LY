using System;
using System.Web;
using System.Web.Routing;

namespace Gods.Web {
	internal class JsWriter : IRouteHandler, IHttpHandler {
		Func<object> processRequest;

		public JsWriter(Func<object> processRequest) {
			this.processRequest = processRequest;
		}

		bool IHttpHandler.IsReusable => false;

		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) => this;

		void IHttpHandler.ProcessRequest(HttpContext context) =>
			context.Response.Write(processRequest?.Invoke());
	}
}