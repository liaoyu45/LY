namespace BLL.V0 {
	public class Work : Gods.AOP.Model, IWork {
		public int GoWork() {
			System.Console.WriteLine(nameof(GoWork));
			return 1;
		}
	}
}
