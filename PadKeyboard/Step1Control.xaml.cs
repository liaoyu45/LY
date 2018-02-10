using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PadKeyboard {
    public partial class Step1Control : Grid {

        [STAThread()]
        public static void Main() {
            var ts = new List<Type> { typeof(Step1Control), typeof(Step2Control), typeof(Step3Control) }.GetEnumerator();
            Func<Window> ow = null;
            ow = () => {
                if (!ts.MoveNext()) {
                    return null;
                }
                var w = new Window {
                    Top = 0,
                    Left = 0,
                    Width = SystemParameters.WorkArea.Width,
                    Height = SystemParameters.WorkArea.Height,
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Background = new SolidColorBrush(),
                    ResizeMode = ResizeMode.NoResize,
                    Content = Activator.CreateInstance(ts.Current)
                };
                w.Closed += delegate {
                    ow()?.Show();
                };
                return w;
            };
            new Application().Run(ow());
        }

        public Step1Control() {
            InitializeComponent();
            for (var i = 0; i < Beard.KeysMax; i++) {
                countGrid.Children.Add(new SquareButton { IsEnabled = false });
            }
            InputDevice d = null, d0 = null, d1 = null;
            var MaxRadius = SystemParameters.WorkArea.Width / 8;
            RoutedEventHandler init = delegate {
                countButton.Content = Beard.KeysCount;
                countGrid.Children.Remove(countButton);
                countGrid.Children.Insert(Beard.KeysCount - 1, countButton);
                var edge = (int)Math.Ceiling(Math.Sqrt(Beard.KeysCount));
                var w = countGrid.ActualWidth / edge;
                Enumerable.Range(0, edge * edge)
                    .Select(i => countGrid.Children[i] as SquareButton)
                    .ToList().ForEach(b => {
                        b.Width = b.Height = w;
                    });
                countButton.FontSize = countButton.Width / 3 * 2;
            };
            Loaded += init;
            countPanel.TouchLeave += (s, e) => {
                if (e.Device != d) {
                    return;
                }
                d = null;
                countButton.EffectOff();
            };
            countPanel.TouchDown += (s, e) => {
                if (d == null) {
                    d = e.Device;
                    countButton.EffectOn();
                }
            };
            countPanel.TouchMove += (s, e) => {
                if (d != e.Device) {
                    return;
                }
                e.Handled = true;
                var left = Elements.Inside(e, countGrid).X;
                countGrid.Margin = new Thickness { Left = left };
                Beard.KeysCount = Beard.KeysMin + (int)(left / (ActualWidth - countGrid.ActualWidth) * Beard.KeysRange);
                init(null, null);
            };
            addPanel.TouchMove += (s, e) => {
                var f = e.Device == d0 ? f0 : e.Device == d1 ? f1 : null;
                if (f == null) {
                    return;
                }
                var fa = f == f0 ? f1 : f0;
                var p = Elements.Inside(e, f);
                f.Margin = new Thickness { Left = p.X, Top = p.Y };
                fa.Width = fa.Height = f.Height = f.Width = Math.Min(ActualWidth / 8, (new Point(f.Margin.Left, f.Margin.Top) - new Point(fa.Margin.Left, fa.Margin.Top)).Length);
            };
            addPanel.TouchLeave += (s, e) => {
                if (e.Device == d0) {
                    d0 = null;
                } else if (e.Device == d1) {
                    d1 = null;
                } else {
                    return;
                }
                if ((d0 ?? d1) == null && f1.Width < MaxRadius) {
                    Beard.Radius = f1.Width / 2;
                    (Parent as Window).Close();
                }
            };
            addPanel.TouchDown += (s, e) => {
                if (d0 == null) {
                    d0 = e.Device;
                } else {
                    d1 = e.Device;
                }
            };
        }
    }
}
