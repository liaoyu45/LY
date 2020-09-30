using System;
using System.Windows;

namespace TouchKeyboard {
	class Settings {
		public static double Radius;
		public static double Diameter => Radius * 2;
		public static Rect WorkArea => new Rect(Radius, Radius, SystemParameters.PrimaryScreenWidth - Diameter, SystemParameters.PrimaryScreenHeight - Diameter);

		public static Point[] Points;
	}
}
