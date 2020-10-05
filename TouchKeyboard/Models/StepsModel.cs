using System.Linq;
using System.Windows;

namespace TouchKeyboard.Models {
	public class StepsModel : NotifyBase {
		public static StepsModel Context { get; } = new StepsModel();

		private double radius;

		public double Radius {
			get { return radius; }
			set {
				radius = value;
				Notify(nameof(Radius));
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Finger> Fingers { get; } = new System.Collections.ObjectModel.ObservableCollection<Finger>();

		public void Attach(int id, Point p) {
			var f = Fingers.FirstOrDefault(a => (a.Point - p).Length < Radius && a.Released) ?? new Finger();
			f.Point = p;
			f.Released = false;
			f.Margin = new Thickness(p.X - Radius, p.Y - Radius, 0, 0);
			if (f.Id == 0) {
				if (Fingers.Count > 1 && Radius == 0) {
					Fingers.Clear();
					return;
				}
				Fingers.Add(f);
			}
			f.Id = id;
		}

		public void AdjustFinger(int id, Point p) {
			if (Fingers.FirstOrDefault(a => a.Id == id && !a.Released) is Finger f) {
				f.Point = p;
				if (Fingers.Count == 2 && Fingers.Count(a => !a.Released) == 2) {
					Radius = (Fingers[0].Point - Fingers[1].Point).Length / 2;
				}
				f.Margin = new Thickness(p.X - Radius, p.Y - Radius, 0, 0);
			}
		}

		public void FinishSettingFinger(int id) {
			if (Fingers.FirstOrDefault(a => a.Id == id && !a.Released) is Finger f) {
				f.Released = true;
				if (Fingers.Any(a => a != f && (a.Point - f.Point).Length < Radius * 2)) {
					Fingers.Remove(f);
				}
			}
		}
	}
}
