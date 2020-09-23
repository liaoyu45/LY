using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace TouchKeyboard.Models {
	public class Step0Model : NotifyBase {
		public static Step0Model DataContext { get; } = new Step0Model();

		private int s = (int)Math.Sqrt(Beard.KeysMin);
		private int kc;
		private double r = 44;

		public int Size {
			get { return s; }
			set {
				s = value;
				Notify(nameof(Size));
			}
		}

		public double Diameter => Radius * 2;

		public double Radius {
			get { return r; }
			set {
				r = value;
				Notify(nameof(Radius));
				Notify(nameof(Diameter));
			}
		}

		public int KeysCount {
			get { return kc; }
			set {
				kc = value;
				Size = (int)Math.Ceiling(Math.Sqrt(kc));
				foreach (var item in Items) {
					item.Visible = item.Index == kc - 1;
				}
			}
		}

		public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>(Enumerable.Range(0, Beard.KeysMax).Select(e => new Item(e)));

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
			(fingers.FirstOrDefault(a => a.Id == 0) ?? new Finger()).Id = id;
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

		public void SetKeysCount(double addedPercent) {
			KeysCount = Beard.KeysMin + (int)(addedPercent * Beard.KeysRange);
		}
	}
}
