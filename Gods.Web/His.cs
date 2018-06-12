using System.IO;

namespace Gods.Web {
	public class His {
		public string Implements { get; set; } = "bin";
		public string Validators { get; set; } = "bin";
		public string Modules { get; set; } = "bin";
		public string AjaxKey { get; set; } = "Him1344150689";//nameof(Him) + nameof(Him).GetHashCode()
		public string AjaxRoute { get; set; } = "Gods";
		public bool AllowCache { get; set; } = true;

		internal void SetRoot(string webRoot) {
			Directory.CreateDirectory(webRoot + "/Scripts/" + AjaxRoute);
			File.WriteAllText($"{webRoot}/Scripts/{AjaxRoute}/{AjaxRoute}.js", Properties.Resources.Map.Replace(nameof(AjaxRoute), AjaxRoute));
			Implements = webRoot + Implements;
			Validators = webRoot + Validators;
			Modules = webRoot + Modules;
		}
	}
}
