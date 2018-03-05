using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PadKeyboard {
    public partial class SquareButton : Button {
        public SquareButton() : this(true) { }
        public SquareButton(bool auto) {
            InitializeComponent();
            duration = TimeSpan.FromMilliseconds(222);
            old = (gs2 = (FindResource("gs") as GradientStopCollection)[2]).Offset;
            if (!auto) {
                return;
            }
            EventHandler<TouchEventArgs> down = null;
            TouchDown += down = (s, e) => {
                EffectOn();
                TouchDown -= down;
                var p = Parent as FrameworkElement;
                while (p.Parent is FrameworkElement) {
                    p = p.Parent as FrameworkElement;
                }
                EventHandler<TouchEventArgs> leave = null;
                leave = (ss, ee) => {
                    if (ee.Device != e.Device) {
                        return;
                    }
                    EffectOff();
                    TouchDown += down;
                    p.TouchLeave -= leave;
                };
                p.TouchLeave += leave;
            };
        }

        public void EffectOn() =>
            gs2.BeginAnimation(GradientStop.OffsetProperty, new DoubleAnimation(1, duration));

        public void EffectOff() =>
            gs2.BeginAnimation(GradientStop.OffsetProperty, new DoubleAnimation(old, duration));

        public void ToggleEffect(bool on) {
            if (on) {
                EffectOn();
            } else {
                EffectOff();
            }
        }

        private readonly double old;
        private readonly TimeSpan duration;
        private readonly GradientStop gs2;
    }
}
