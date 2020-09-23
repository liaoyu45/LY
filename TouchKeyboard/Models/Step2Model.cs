using System.Windows;

namespace TouchKeyboard.Models {
	public class Step2Model {
		public static Step2Model DataContext { get; } = new Step2Model();
		public class Finger {
			public Point Point { get; set; }
			public int Index { get; set; }
		}
	}
}
