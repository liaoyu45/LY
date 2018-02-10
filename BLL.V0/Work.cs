using System.ComponentModel.Composition;

namespace BLL.V0 {
	[Export(typeof(IWork))]
	public class Work : M, IWork {
		[Gods.Web.Cache(-1)]
		public int GoWork() {
			System.Console.WriteLine(nameof(GoWork));
			return 1;
		}
	}
}
