using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PadKeyboard {
    public partial class DanymicKeyboard : Window {
        private Brush defaultColor;
        private Brush hoverColor;

        public DanymicKeyboard() {
            InitializeComponent();
            defaultColor = Resources["defaultColor"] as Brush;
            hoverColor = Resources["hoverColor"] as Brush;
            TouchDown += addBendFingers;
            weight.ValueChanged += delegate {
                var nw = weight.Value;
                foreach (var item in path.Children.OfType<Ellipse>()) {
                    item.Width = nw;
                    item.Height = nw;
                    item.Margin = new Thickness {
                        Left = item.Margin.Left + (w - nw) / 2,
                        Top = item.Margin.Top + (w - nw) / 2
                    };
                }
                w = nw;
            };
            LostFocus += delegate {
                path.Children.RemoveRange(1, 132);

            };
        }

        double w = 88;
        private void addBendFingers(object sender, TouchEventArgs e) {
            var p = e.GetTouchPoint(this).Position;
            var ell = new Ellipse {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness { Left = p.X - w / 2, Top = p.Y - w / 2 },
                Width = w,
                Height = w,
                Fill = defaultColor
            };
            path.Children.Add(ell);
            if (path.Children.Count == 11) {
                TouchDown -= addBendFingers;
                foreach (var item in path.Children.OfType<Ellipse>()) {
                    item.TouchDown += Item_TouchDown;
                }
            }
        }

        private void Item_TouchUp(object sender, TouchEventArgs e) {
            var s = sender as Ellipse;
            if (s.Tag == null) {
                return;
            }
            s.Tag = null;
            s.TouchMove -= DanymicKeyboard_TouchMove;
            s.TouchUp -= Item_TouchUp;
            s.TouchDown += Item_TouchDown;
        }

        private void Item_TouchDown(object sender, TouchEventArgs e) {
            var s = sender as Ellipse;
            if (s.Tag != null) {
                return;
            }
            var p = e.GetTouchPoint(this).Position;
            s.Margin = new Thickness { Left = p.X - w / 2, Top = p.Y - w / 2 };
            s.Tag = new object[] { s.Margin, p, e.Device };
            s.TouchMove += DanymicKeyboard_TouchMove;
            s.TouchUp += Item_TouchUp;
            s.TouchLeave += Item_TouchUp;
            s.TouchDown -= Item_TouchDown;
        }

        private void DanymicKeyboard_TouchMove(object sender, TouchEventArgs e) {
            var s = sender as Ellipse;
            if (s.Tag == null) {
                return;
            }
            var t = (object[])s.Tag;
            if (e.Device != t[2] as InputDevice) {
                return;
            }
            var om = (Thickness)t[0];
            var op = (Point)t[1];
            var p = e.GetTouchPoint(this).Position;
            s.Margin = new Thickness { Left = om.Left + p.X - op.X, Top = om.Top + p.Y - op.Y };
        }
    }
}