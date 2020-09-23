using System;
using System.Windows;
using System.Windows.Input;

namespace TouchKeyboard {
	public static class Utils {
		public static Point InsideWindow(this TouchEventArgs self, FrameworkElement e) {
			return InsideWindow(self, e.RenderSize);
		}

		public static Point InsideWindow(this TouchEventArgs self, Size s) {
			var p = self.GetTouchPoint(Application.Current.MainWindow).Position;
			return new Point(Math.Max(0, Math.Min(SystemParameters.PrimaryScreenWidth - s.Width, p.X - s.Width / 2)), Math.Max(0, Math.Min(SystemParameters.PrimaryScreenHeight - s.Height, p.Y - s.Height / 2)));
		}

		public static void SetDragMove(this FrameworkElement element, bool? xy = default) {
			if (xy != false) {
				element.HorizontalAlignment = HorizontalAlignment.Left;
			}
			if (xy != true) {
				element.VerticalAlignment = VerticalAlignment.Top;
			}
			InputDevice d = default;
			element.TouchDown += (s, e) => {
				if (d != null) {
					return;
				}
				d = e.TouchDevice;
				var w = Application.Current.MainWindow;
				var ps = element.Parent as FrameworkElement ?? new FrameworkElement();
				var m = element.Margin;
				var p = e.GetTouchPoint(ps).Position;
				var mle = ps.ActualWidth - element.ActualWidth;
				var mto = ps.ActualHeight - element.ActualHeight;
				void p1(object s, TouchEventArgs e) {
					if (e.TouchDevice != d) {
						return;
					}
					var pp = e.GetTouchPoint(ps).Position;
					var left = xy != false ? Math.Max(0, Math.Min(mle, m.Left + pp.X - p.X)) : m.Left;
					var top = xy != true ? Math.Max(0, Math.Min(mto, m.Top + pp.Y - p.Y)) : m.Top;
					element.Margin = new Thickness(left, top, 0, 0);
				}
				void p2(object sender, TouchEventArgs e) {
					if (e.TouchDevice != d) {
						return;
					}
					d = null;
					w.TouchMove -= p1;
					w.TouchUp -= p2;
				}
				w.TouchMove += p1;
				w.TouchUp += p2;
			};
		}
	}
}
