using System.Windows;

namespace TouchKeyboard.Models {
	public class Finger : NotifyBase {
		public Point Point { get; set; }
		private Thickness margin;

		public int Id { get; set; }

		public Thickness Margin {
			get => margin; set {
				margin = value;
				Notify(nameof(Margin));
			}
		}

		public bool Released { get; set; }
	}
}