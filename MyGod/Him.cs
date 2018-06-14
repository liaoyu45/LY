namespace MyGod {
	public class Him : World.I {
		public int GetLuck(string thing) {
			return thing.GetHashCode();
		}
		public int GetLuck(int i) {

		}
	}
}
