using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    public partial class FRRREEE : Window {

        private Color randomColor;
        private Color red;
        private double width;
        int maxCount = 36;//max count

        public FRRREEE() {
            InitializeComponent();
            var r = new Random();
            width = 55;
            randomColor = new Color { A = byte.MaxValue, R = 0, G = (byte)r.Next(byte.MaxValue), B = (byte)r.Next(byte.MaxValue) };
            red = new Color { A = byte.MaxValue, R = byte.MaxValue, B = 0, G = 0 };
            settings.Width = settings.Height = width;
            settings.Fill = getBrush(false);
            settingsOutter.Width = settingsOutter.Height = width * 2;
            settingsOutter.Fill = getBrush(true);
            addKeys();
            setNumbers(0, maxCount);
        }


        private RadialGradientBrush getBrush(bool d) {
            var g = new RadialGradientBrush();
            g.GradientStops.Add(new GradientStop { Color = d ? randomColor : red, Offset = 0 });
            g.GradientStops.Add(new GradientStop { Color = d ? red : randomColor, Offset = 0 });
            g.GradientStops.Add(new GradientStop { Color = d ? randomColor : red, Offset = 1 });
            return g;
        }

        private void addKeys() {
            var ds = new Dictionary<InputDevice, Ellipse>();
            var configrating = false;
            var changed = 0;
            Action addSettings = null, removeSettings = null, startToAddKeys = null, stopAddingKeys = null;
            EventHandler<TouchEventArgs>
                startToMoveKey = null,
                moveKey = null,
                confirmKey = null,
                addKey = null,
                remove = null,
                stopChange = null,
                startChange = null,
                changeSize = null;
            RoutedEventHandler lost = null;
            remove = (s, e) => {
                var el = s as Ellipse;
                if (el.Tag != null) {
                    return;
                }
                var s1 = (el.Fill as RadialGradientBrush).GradientStops[1];
                if (s1.Offset == 1) {
                    s1.Offset = 0;
                }
            };
            startToMoveKey = (s, e) => {
                var ell = s as Ellipse;
                if ((ell.Fill as RadialGradientBrush).GradientStops[1].Offset == 1) {
                    return;
                }
                if (ds.ContainsKey(e.Device)) {
                    return;
                }
                removeSettings();
                var p = e.GetTouchPoint(this).Position;
                ell.Margin = new Thickness { Left = p.X - ell.Width / 2, Top = p.Y - ell.Width / 2 };
                ell.Tag = new Point(ell.Margin.Left - p.X, ell.Margin.Top - p.Y);
                ds.Add(e.Device, ell);
            };
            moveKey = (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var ell = ds[e.Device];
                if (!(ell.Tag is Point)) {
                    return;
                }
                var t = (Point)ell.Tag;
                var p = e.GetTouchPoint(this).Position;
                var sr = (ell.Fill as RadialGradientBrush).GradientStops[1];
                if (sr.Offset < 1) {
                    sr.Offset = Math.Min(1, sr.Offset + 0.01);
                }
                ell.Margin = new Thickness { Left = t.X + p.X, Top = t.Y + p.Y };
            };
            confirmKey = (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var ell = ds[e.Device];
                ds.Remove(e.Device);
                ell.Tag = null;
                var s1 = (ell.Fill as RadialGradientBrush).GradientStops[1];
                if (s1.Offset != 1) {
                    s1.Offset = 0;
                }
                if (ds.Count == 0) {
                    addSettings();
                }
            };
            addKey = (s, e) => {
                if (keysPanel.Children.Count >= maxCount) {
                    return;
                }
                if (ds.ContainsKey(e.Device)) {//why
                    return;
                }
                var p = e.GetTouchPoint(this).Position;
                if ((new Point(Width / 2, Height / 2) - p).Length < settings.Width / 2) {
                    return;
                }
                foreach (var item in keysPanel.Children.Cast<Ellipse>()) {
                    if ((new Point { X = item.Margin.Left + width / 2, Y = item.Margin.Top + width / 2 } - p).Length < width / 2) {
                        return;
                    }
                }
                removeSettings();
                var ell = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - width / 2, Top = p.Y - width / 2 },
                    Width = width,
                    Height = width,
                    Fill = getBrush(true),
                    Tag = new Point(-width / 2, -width / 2)
                };
                ds.Add(e.Device, ell);
                keysPanel.Children.Add(ell);
                ell.TouchDown += startToMoveKey;
                ell.TouchUp += remove;
                setNumbers(keysPanel.Children.Count, maxCount);
            };
            startToAddKeys = delegate {
                TouchDown += addKey;
                TouchMove += moveKey;
                TouchUp += confirmKey;
                TouchLeave += confirmKey;
            };
            stopAddingKeys = delegate {
                TouchDown -= addKey;
                TouchMove -= moveKey;
                TouchUp -= confirmKey;
                TouchLeave -= confirmKey;
            };
            stopChange = delegate {
                lost(null, null);
            };
            changeSize = (s, e) => {
                if (!configrating) {
                    return;
                }
                var p = e.GetTouchPoint(this).Position;
                var v = p - new Point(Width / 2, Height / 2);
                (settingsOutter.Fill as RadialGradientBrush).GradientStops[1].Offset = Math.Min(1, v.Length / settings.Width);
                if (v.Length > settings.Width) {
                    changed = (int)(v.Length / settings.Width - 1) * (v.X > 0 ? 1 : -1);
                    var maxCanBe = Math.Max(0, maxCount + changed);
                    var left = Math.Min(maxCanBe, keysPanel.Children.Count);
                    for (var i = 0; i < keysPanel.Children.Count; i++) {
                        keysPanel.Children[i].Visibility = i < left ? Visibility.Visible : Visibility.Hidden;
                    }
                    setNumbers(left, maxCanBe);
                    return;
                }
                var w = width + (v.X > 0 ? 1 : -1);
                if (w < settings.Width) {
                    w = settings.Width;
                }
                if (w > Height / 6) {
                    w = Height / 6;
                }
                width = w;
                foreach (var item in keysPanel.Children.Cast<Ellipse>()) {
                    item.Margin = new Thickness { Left = item.Margin.Left + (item.Width - width) / 2, Top = item.Margin.Top + (item.Width - width) / 2 };
                    item.Width = item.Height = width;
                }
            };
            lost = delegate {
                if (!configrating) {
                    return;
                }
                configrating = false;
                (settingsOutter.Fill as RadialGradientBrush).GradientStops[1].Offset = 0;
                maxCount += changed;
                if (maxCount < 0) {
                    maxCount = 0;
                }
                keysPanel.Children.RemoveRange(maxCount, int.MaxValue);
                setNumbers(keysPanel.Children.Count, maxCount);
                TouchMove -= changeSize;
                TouchLeave -= stopChange;
                TouchUp -= stopChange;
                LostFocus -= lost;
                startToAddKeys();
            };
            startChange = (s, e) => {
                if (configrating) {
                    return;
                }
                configrating = true;
                changed = 0;
                stopAddingKeys();
                TouchMove += changeSize;
                TouchUp += stopChange;
                TouchLeave += stopChange;
                LostFocus += lost;
            };
            addSettings = delegate {
                settings.TouchDown += startChange;
                count_max.TouchDown += startChange;
            };
            removeSettings = delegate {
                settings.TouchDown -= startChange;
                count_max.TouchDown -= startChange;
            };
            addSettings();
            startToAddKeys();
        }

        private void setNumbers(int now, int max) {
            count_max.Text = now + "/" + max;
        }

        private void step2() {

        }
    }
}