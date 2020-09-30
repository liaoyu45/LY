using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace TouchKeyboard.Models {
	public class Step0Model : NotifyBase {
		public static Step0Model DataContext { get; } = new Step0Model();

		private double radius = 44;

		public double Diameter => Radius * 2;

		public double Radius {
			get { return radius; }
			set {
				radius = value;
				Notify(nameof(Radius));
				Notify(nameof(Diameter));
			}
		}

		public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

		public class Item : NotifyBase {
			private bool visible;

			public Item(int index) {
				Index = index;
			}

			public bool Visible {
				get => visible; set {
					visible = value;
					Notify(nameof(Visible));
				}
			}

			public int Index { get; }
			public override string ToString() {
				return (Index + 1).ToString();
			}
		}

		public void SetRadius(int id, Point point) {
			var f = fingers.FirstOrDefault(a => a.Id == id) ?? new Finger();
			f.Id = id;
			f.Point = point;
			if (fingers.All(a => a.Id != 0)) {
				Radius = (fingers[0].Point - fingers[1].Point).Length / 2;
				foreach (var item in fingers) {
					item.Margin = new Thickness(item.Point.X - Radius, item.Point.Y - Radius, 0, 0);
				}
				Notify(nameof(Margin0));
				Notify(nameof(Margin1));
			}
		}

		public void AttachFinger(int id) {
			if (!fingers.Any(a => a.Id == id)) {
				(fingers.FirstOrDefault(a => a.Id == 0) ?? new Finger()).Id = id;
			}
		}

		public void DettachFinger(int id) {
			(fingers.FirstOrDefault(a => a.Id == id) ?? new Finger()).Id = 0;
		}

		private readonly Finger[] fingers = new[] { new Finger(), new Finger() };
		public Thickness Margin0 => fingers[0].Margin;
		public Thickness Margin1 => fingers[1].Margin;

		private class Finger : NotifyBase {
			public int Id { get; set; }
			public Point Point { get; set; }
			public Thickness Margin { get; set; }
		}
	}
}
