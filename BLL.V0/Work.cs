using Gods.AOP;

namespace BLL.V0 {
	public class Work : Model, IWork {
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
