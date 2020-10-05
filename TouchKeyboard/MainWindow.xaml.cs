using System.Windows;

namespace TouchKeyboard {
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			var ctx = Models.StepsModel.Context;
			TouchDown += (s, e) => ctx.Attach(e.Device.GetHashCode(), e.GetTouchPoint(this).Position);
			TouchMove += (s, e) => { ctx.AdjustFinger(e.Device.GetHashCode(), e.GetTouchPoint(this).Position); };
			TouchLeave += (s, e) => { ctx.FinishSettingFinger(e.Device.GetHashCode()); };
		}
	}
}
