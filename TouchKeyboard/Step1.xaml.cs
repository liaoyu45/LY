using System.Linq;
using System.Windows;

namespace TouchKeyboard {
	public partial class Step1 : Window {
		public Step1() {
			InitializeComponent();
			var ctx = Models.Step1Model.DataContext;
			Steps.SlideWindow<Step2>(this, delegate {
				Settings.Points = ctx.Points.Select(a => new Point(a.Left + Settings.Radius, a.Top + Settings.Radius)).ToArray();
			});
			TouchDown += (s, e) => {
				ctx.AttachFinger(e.Device.GetHashCode(), e.GetTouchPoint(this).Position);
			};
			TouchMove += (s, e) => {
				ctx.MoveFinger(e.Device.GetHashCode(), e.GetTouchPoint(this).Position);
			};
			TouchLeave += (s, e) => {
				ctx.ReleaseFinger(e.Device.GetHashCode());
			};
		}
	}
}
