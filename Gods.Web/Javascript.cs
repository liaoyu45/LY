using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Gods.Web {
	public static class Javascript {
		internal static void LoadDllInBin(string path, Action<Assembly> dowith) {
			var fs = Directory.GetFiles(path, "*.dll");
			foreach (var item in fs) {
				try {
					var a = Assembly.LoadFrom(item);
					dowith(a);
				} catch {
				}
			}
		}
		public static void WriteLikeSCharp() {
			WriteLikeSCharp("CSharpMap", HostingEnvironment.ApplicationPhysicalPath + "/bin", "window", nameof(Gods) + Guid.NewGuid().GetHashCode());
		}
		public static void WriteLikeSCharp(string relativePath, string bllPath, string ThisArg, string AjaxKey) {
			if (AsyncPage.WebApp != null) {
				throw new Exception("ever called");
			}
			AsyncPage.AjaxKey = AjaxKey;
			AsyncPage.WebApp = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Assembly;
			var folder = HostingEnvironment.MapPath("/" + relativePath);
			Directory.CreateDirectory(folder);
			var all = AsyncPage.WebApp.DefinedTypes.Where(t => t.IsSubclassOf(typeof(AsyncPage))).Select(t => Activator.CreateInstance(t)).Cast<AsyncPage>().ToList();
			if (all.Count == 0) {
				all.Add(new AsyncPage());
			}
			foreach (var item in all) {
				var CSharp = new JObject();
				LoadDllInBin(bllPath, a => {
					if (a.FullName == AsyncPage.WebApp.FullName) {
						return;
					}
					var c = a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
					if (c.StartsWith(nameof(Microsoft)) || c.StartsWith(nameof(Gods)) || c.StartsWith(nameof(Newtonsoft))) {
						return;
					}
					if (!item.FilterAssembly(a)) {
						return;
					}
					foreach (var t in a.ExportedTypes) {
						if (!t.IsInterface) {
							AsyncPage.cache[item.GetType().GUID] = t.GetMethods().Where(m => !m.IsSpecialName).ToDictionary(m => m.GetHashCode().ToString());
							CSharp.Append(t, item.FilterType, item.FilterMethod);
						}
					}
				});
				File.WriteAllText($"{folder}/{item.GetType().Name}.js",
					Properties.Resources.Map
					.Replace(nameof(ThisArg), ThisArg)
					.Replace(nameof(AjaxKey), AjaxKey)
					.Replace(nameof(CSharp), CSharp.ToString()));
			}
		}
	}
}
