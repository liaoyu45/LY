using System;
using System.Windows;

namespace TouchKeyboard {
	class Steps {
		public static void SlideWindow<Next>(Window w, Action then = default) where Next : Window {
			w.IsManipulationEnabled = true;
			w.ManipulationCompleted += (s, e) => {
				if (e.TotalManipulation.Translation is Vector v && v.X / v.Y > 2 && v.X > SystemParameters.PrimaryScreenWidth / 3) {
					then?.Invoke();
					(Application.Current.MainWindow = Activator.CreateInstance<Next>()).Show();
					w.Close();
				}
			};
		}
	}
}
