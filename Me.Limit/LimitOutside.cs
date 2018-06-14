using System.ComponentModel.Composition;

namespace Me.Limit {
	[Export(typeof(World.ILimit<I>))]
	public class SoulLimit : World.ILimit<I> {
		I s;
		public object Desire(string t) {
			var v = (s?.GetHashCode() ?? 0);
			if (v % 20 == 0) {
				return new[] { "oweruwoipruqoieu" };
			}
			return null;
		}
	}
}
