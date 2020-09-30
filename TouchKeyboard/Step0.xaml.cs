using System.Windows;

namespace TouchKeyboard {
	public partial class Step0 : Window {
		public Step0() {
			InitializeComponent();
			var ctx = Models.Step0Model.DataContext;
			Steps.SlideWindow<Step1>(this, delegate {
				Settings.Radius = ctx.Radius;
			});
			TouchDown += (s, e) => ctx.AttachFinger(e.Device.GetHashCode());
			TouchLeave += (s, e) => { ctx.DettachFinger(e.Device.GetHashCode()); };
			TouchMove += (s, e) => { ctx.SetRadius(e.Device.GetHashCode(), e.GetTouchPoint(this).Position); };

			MouseDown += (s, e) => { ctx.AttachFinger(1); ctx.SetRadius(1, e.GetPosition(this)); };
			MouseMove += (s, e) => { ctx.AttachFinger(2); ctx.SetRadius(2, e.GetPosition(this)); };
			MouseUp += delegate { ctx.DettachFinger(2); ctx.DettachFinger(1); };
			MouseLeave += delegate { ctx.DettachFinger(2); ctx.DettachFinger(1); };
		}
	}
}
