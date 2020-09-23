using System.Windows;

namespace TouchKeyboard {
	public partial class Step0 : Window {
		public Step0() {
			InitializeComponent();
			var ctx = Models.Step0Model.DataContext;
			ctx.KeysCount = Settings.KeysCount;
			Steps.SlideWindow<Step1>(this, delegate {
				Settings.Radius = ctx.Radius;
				Settings.KeysCount = ctx.KeysCount;
			});
			System.Windows.Input.InputDevice d = default;
			Utils.SetDragMove(countPanel, true);
			countPanel.TouchDown += (s, e) => d ??= e.Device;
			addPanel.TouchDown += (s, e) => ctx.AttachFinger(e.Device.GetHashCode());
			TouchLeave += (s, e) => {
				d = e.Device == d ? null : d;
				ctx.DettachFinger(e.Device.GetHashCode());
			};
			TouchMove += (s, e) => {
				if (d == e.Device) {
					ctx.SetKeysCount(countPanel.Margin.Left / (ActualWidth - countPanel.ActualWidth));
				} else {
					ctx.SetRadius(e.Device.GetHashCode(), e.GetTouchPoint(this).Position); 
				}
			};
		}
	}
}
