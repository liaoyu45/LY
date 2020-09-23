using System;
using System.Windows;

namespace TouchKeyboard {
	class Settings {
		public static int KeysCount = Beard.KeysMin;
		public static double Radius;
		public static double Diameter => Radius * 2;
		public static Rect WorkArea => new Rect(Radius, Radius, SystemParameters.PrimaryScreenWidth - Diameter, SystemParameters.PrimaryScreenHeight - Diameter);

		public static Point[] Points;

		public static Point InsideWorkArea(Point p) {
			return new Point(Math.Max(WorkArea.Left, Math.Min(WorkArea.Right, p.X)), Math.Max(WorkArea.Top, Math.Min(WorkArea.Bottom, p.Y)));
		}
	}
}
