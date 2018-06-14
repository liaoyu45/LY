using System.ComponentModel.Composition;

namespace Me.World {
	[Export(typeof(World.World<Soul>))]
	public class SoulLimit : Who.World<Soul> {
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
