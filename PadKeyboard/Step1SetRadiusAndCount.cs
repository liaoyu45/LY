using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Math;

namespace PadKeyboard {
    internal class Step1SetRadiusAndCount : KeyStep {

        private Grid addPanel = Elements.BgA1Grid();
        private Grid effectPanel = new Grid();
        private Grid countPanel = new Grid {
            Background = new SolidColorBrush(new Color { A = 1 }),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Height = Height * .382
        };
        private WrapPanel countGrid = new WrapPanel {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Left,
            Height = Height * .382,
            Width = Height * .382
        };

        private double r = Beard.Radius;
        private int c = Beard.KeysCount;
        private TextBlock countText = new TextBlock {
            FontSize = 1,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        public Step1SetRadiusAndCount() {
            Content.Children.Add(effectPanel);
            Content.Children.Add(addPanel);
            Content.Children.Add(countPanel);
            countPanel.Children.Add(countGrid);
            for (var i = 0; i < Beard.KeysMax; i++) {
                countGrid.Children.Add(ShadowBox.SquareButton());
            }
            setColor();
            var fs = new List<finger>();
            var d = default(InputDevice);
            var maxLeft = Width - countGrid.Width;
            countPanel.TouchLeave += (s, e) => {
                if (e.Device != d) {
                    return;
                }
                d = null;
            };
            countPanel.TouchMove += (s, e) => {
                e.Handled = true;
                if (d != null && d != e.Device) {
                    return;
                }
                d = e.Device;
                var left = Inside(e, countGrid).X;
                countGrid.Margin = new Thickness { Left = left };
                c = Beard.KeysMin + (int)(left / maxLeft * Beard.KeysRange);
                setColor();
            };
            addPanel.TouchMove += (s, e) => {
                var f0 = fs.FirstOrDefault(p => p.d == e.Device);
                if (f0 == null) {
                    return;
                }
                f0.p = Inside(e, f0.e);
                if (fs.Count == 2) {
                    var f1 = fs.First(f => f != f0);
                    var _r = Min(MaxRadius, (f0.p - f1.p).Length / 2);
                    if (_r > 0) {
                        r = _r;
                    }
                    f0.e.Width = f1.e.Width = f0.e.Height = f1.e.Height = r * 2;
                    setColor();
                }
                f0.e.Margin = new Thickness { Left = f0.p.X, Top = f0.p.Y };
            };
            addPanel.TouchLeave += (s, e) => {
                var f = fs.FirstOrDefault(p => p.d == e.Device);
                if (f == null) {
                    return;
                }
                fs.Remove(f);
                effectPanel.Children.Remove(f.e);
            };
            addPanel.TouchDown += (s, e) => {
                if (fs.Count == 2) {
                    return;
                }
                var p = e.GetTouchPoint(addPanel).Position;
                var ell = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - r, Top = p.Y - r },
                    Width = r * 2,
                    Height = r * 2,
                    Fill = new SolidColorBrush(Colors.Black)
                };
                fs.Add(new finger { d = e.Device, e = ell, p = p });
                effectPanel.Children.Add(ell);
            };
        }

        protected override int Finish() {
            Beard.Radius = r;
            Beard.KeysCount = c;
            return 1;
        }

        private void setColor() {
            var edge = Ceiling(Sqrt(c));
            if (countText.Parent != null) {
                ((Grid)countText.Parent).Children.Remove(countText);
            }
            countText.Text = c + string.Empty;
            ((((Grid)(countGrid.Children[c - 1])).Background as VisualBrush).Visual as Grid).Children.Add(countText);
            for (var i = 0; i < countGrid.Children.Count; i++) {
                var g = countGrid.Children[i] as Grid;
                g.Width = g.Height = countGrid.Width / edge;
            }
        }

        class finger {
            public InputDevice d;
            public Ellipse e;
            public Point p;
        }
    }
}