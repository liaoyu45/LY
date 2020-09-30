using System.Linq;
using System.Windows;

namespace TouchKeyboard.Models {
	public class Step1Model {
		public static Step1Model DataContext { get; } = new Step1Model();
		public const int Lines = 6;
		public System.Collections.ObjectModel.ObservableCollection<Thickness> Points { get; } = new System.Collections.ObjectModel.ObservableCollection<Thickness>();

		public void AttachFinger(int id, Point p) {
			if (Fingers.FirstOrDefault(a => (a.Point - p).Length < Settings.Diameter) is Finger ff) {
				ff.Id = id;
				ff.Point = p;
				return;
			}
			Fingers.Add(new Finger { Id = id, Point = p });
		}

		public void MoveFinger(int id, Point p) {
			if (Fingers.FirstOrDefault(a => a.Id == id) is Finger f) {
				f.Point = p;
			}
		}

		public void ReleaseFinger(int id) {
			if (Fingers.FirstOrDefault(a => a.Id == id) is Finger f) {
				f.Id = 0;
				if (Fingers.Any(a => a != f && (a.Point - f.Point).Length < Settings.Diameter)) {
					Fingers.Remove(f);
				}
				var i = 0;
				Fingers.GroupBy(a => (int)a.Point.Y / (int)(SystemParameters.PrimaryScreenHeight / Lines)).OrderBy(a => a.Key).ToList().ForEach(a => {
					a.OrderBy(a => a.Point.X).ToList().ForEach(a => a.Index = i++);
				});
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Finger> Fingers { get; } = new System.Collections.ObjectModel.ObservableCollection<Finger>();

		public class Finger : NotifyBase {
			public Point Point {
				get => point; set {
					point = value;
					Notify(nameof(Margin));
				}
			}
			private Point point;
			private int index;

			public int Id { get; set; }
			public Thickness Margin => new Thickness(Point.X - Settings.Radius, Point.Y - Settings.Radius, 0, 0);
			public int Index {
				get => index; set {
					index = value;
					Notify(nameof(Index));
				}
			}
			public override string ToString() {
				return (Index + 1).ToString();
			}
		}
	}
}