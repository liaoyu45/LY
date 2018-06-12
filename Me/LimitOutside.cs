using System.ComponentModel.Composition;

namespace Me {
	[Export(typeof(Limit<Soul>))]
	public class SoulLimit : Limit<Soul> {
		Soul s;
		public object Desire(string t) {
			var v = (s?.GetHashCode() ?? 0);
			if (v % 20 == 0) {
				return new[] { "oweruwoipruqoieu" };
			}
			return null;
		}
	}
}
