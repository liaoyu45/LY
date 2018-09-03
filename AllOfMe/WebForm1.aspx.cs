using System;
using System.Collections.Generic;
using System.Web;

namespace AllOfMe {
	public partial class WebForm1 : System.Web.UI.Page {
		public override void ProcessRequest(HttpContext context) {
			if (context.Request["a"]?.Length > 0) {
				var bs = new byte[context.Request.ContentLength];
				context.Request.InputStream.Read(bs, 0, bs.Length);
				var o = Newtonsoft.Json.JsonConvert.SerializeObject(System.Text.Encoding.UTF8.GetString(bs));
				Console.WriteLine(bs);
			} else {
				base.ProcessRequest(context);
			}
		}
	}
	class Man {
		public string Name { get; set; }
		public string Intro { get; set; }
		public List<Paper> Papers { get; set; } = new List<Paper>();
	}

	public class Paper {
		public string Words { get; set; }
		public string Tag { get; set; }
	}
}