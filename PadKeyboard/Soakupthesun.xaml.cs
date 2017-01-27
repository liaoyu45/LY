using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    public partial class Soakupthesun : Window {

        private Color randomColor;
        private Color red;
        private double radius;
        private readonly double minRadius = 55;
        private readonly double maxRadius = 555;

        public Soakupthesun() {
            InitializeComponent();
            var r = new Random();
            minRadius = 55;
            maxRadius = 555;
            radius = 55;
            randomColor = new Color { A = byte.MaxValue, R = 0, G = (byte)r.Next(byte.MaxValue), B = (byte)r.Next(byte.MaxValue) };
            red = new Color { A = byte.MaxValue, R = byte.MaxValue, B = 0, G = 0 };
            step0();//TODO: 要完全自定义按钮数量！
        }


        private RadialGradientBrush getBrush(bool d) {
            var g = new RadialGradientBrush();
            g.GradientStops.Add(new GradientStop { Color = d ? randomColor : red, Offset = 0 });
            g.GradientStops.Add(new GradientStop { Color = d ? red : randomColor, Offset = 0 });
            g.GradientStops.Add(new GradientStop { Color = d ? randomColor : red, Offset = 1 });
            return g;
        }

        private void step0() {
            var es = new List<Ellipse>();
            EventHandler<TouchEventArgs> addFinger = null;
            addFinger = (s, e) => {
                var p = e.GetTouchPoint(this).Position;
                var ell = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - radius / 2, Top = p.Y - radius / 2 },
                    Width = radius,
                    Height = radius,
                    Fill = getBrush(true)
                };
                es.Add(ell);
                blame.Children.Add(ell);
                if (es.Count == 10) {
                    TouchDown -= addFinger;
                    step1(es.ToArray());
                }
            };
            TouchDown += addFinger;
        }

        private void step1(Ellipse[] es) {
            var theBall = modifyRadius(delegate {
                foreach (var item in es) {
                    var diff = item.Width / 2 - radius / 2;
                    item.Width = radius;
                    item.Height = radius;
                    item.Margin = new Thickness { Left = item.Margin.Left + diff, Top = item.Margin.Top + diff };
                }
            });
            var ds = new Dictionary<InputDevice, Ellipse>();
            EventHandler<TouchEventArgs> start = null;
            EventHandler<TouchEventArgs> move = null;
            EventHandler<TouchEventArgs> stop = null;
            start = (s, e) => {
                var f = s as Ellipse;
                var p = e.GetTouchPoint(this).Position;
                f.Margin = new Thickness { Left = p.X - radius / 2, Top = p.Y - radius / 2 };
                f.Tag = new object[] { f.Margin, p };
                f.TouchDown -= start;
                ds.Add(e.Device, f);
            };
            move = (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var f = ds[e.Device];
                var t = (object[])f.Tag;
                var om = (Thickness)t[0];
                var op = (Point)t[1];
                var p = e.GetTouchPoint(this).Position;
                var sr = (f.Fill as RadialGradientBrush).GradientStops[1];
                if (sr.Offset < 1) {
                    sr.Offset = Math.Min(1, sr.Offset + 0.02);
                }
                f.Margin = new Thickness { Left = om.Left + p.X - op.X, Top = om.Top + p.Y - op.Y };
            };
            stop = (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                if (ds.Count == 10) {
                    if (ds.All(p => (p.Value.Fill as RadialGradientBrush).GradientStops[1].Offset == 1)) {
                        TouchMove -= move;
                        TouchUp -= stop;
                        TouchLeave -= stop;
                        blame.Children.Remove(theBall);
                        step2();
                        return;
                    }
                }
                var f = ds[e.Device];
                (f.Fill as RadialGradientBrush).GradientStops[1].Offset = 0;
                f.Tag = null;
                f.TouchDown += start;
                ds.Remove(e.Device);
            };
            TouchMove += move;
            TouchUp += stop;
            TouchLeave += stop;
            foreach (var item in es) {
                item.TouchDown += start;
            }
        }

        private Ellipse modifyRadius(Action later) {
            var ell = new Ellipse { Width = 66, Height = 66, Fill = getBrush(false) };
            blame.Children.Add(ell);
            var changing = false;
            EventHandler<TouchEventArgs> stopChange = null;
            EventHandler<TouchEventArgs> startChange = null;
            EventHandler<TouchEventArgs> changeSize = null;
            stopChange = delegate {
                changing = false;
                TouchMove -= changeSize;
                TouchLeave -= stopChange;
                TouchUp -= stopChange;
                ell.TouchDown += startChange;
                later();
            };
            changeSize = (s, e) => {
                if (!changing) {
                    return;
                }
                var p = e.GetTouchPoint(this).Position;
                var len = (new Point(Width / 2, Height / 2) - p).Length * 2;
                if (len > maxRadius || len < minRadius) {
                    return;
                }
                radius = ell.Width = ell.Height = len;
            };
            startChange = delegate {
                changing = true;
                ell.TouchDown -= startChange;
                TouchMove += changeSize;
                TouchUp += stopChange;
                TouchLeave += stopChange;
            };
            ell.TouchDown += startChange;
            return ell;
        }

        private void step2() {

        }
    }
}
