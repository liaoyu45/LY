using Gods.AOP;
using System.ComponentModel.Composition;

namespace BLL.V0 {
	[Export(typeof(IWork))]
	public class Work : Model, IWork {
		[Gods.Web.Cache(-1)]
		public int GoWork() {
			System.Console.WriteLine(nameof(GoWork));
			return 1;
		}
		public override string Folder {
			get {
				return @"C:\Users\zy\Source\Repos\LY\AllOfMe\bin";
			}
		}
	}
}
