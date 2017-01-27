using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    public partial class FREE : Window {

        private Color randomColor;
        private Color red = new Color { A = 255, R = 255 };
        private double width;
        private int maxCount = 26;
        private Point center;

        public FREE() {
            InitializeComponent();
            var r = new Random();
            randomColor = new Color { A = 255, R = (byte)r.Next(255), G = (byte)r.Next(255), B = (byte)r.Next(255) };
            center = new Point(SystemParameters.WorkArea.Width / 2, SystemParameters.WorkArea.Height / 2);
            width = SystemParameters.WorkArea.Height / 6 / 2;
            settings.Width = settings.Height = width;
            settings.Fill = getBrush(false);
            settingsOutter.Width = settingsOutter.Height = width * 2;
            settingsOutter.Fill = getBrush(true);
            settings.TouchUp += (s, e) => { };
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
            InputDevice settingsDevice = null;
            var changed = 0;
            Action addSettings = null, removeSettings = null, startToAddKeys = null, stopAddingKeys = null;
            EventHandler<TouchEventArgs>
                tryToMoveKey = null,
                moveKey = null,
                confirmKey = null,
                addKey = null,
                removeOrRestart = null,
                holdAndWait = null,
                stopChange = null,
                startChange = null,
                changeSize = null;
            RoutedEventHandler lost = null;
            Action<TouchEventArgs> startToMoveKey = e => {
                removeSettings();
                var ell = e.Source as Ellipse;
                var p = e.GetTouchPoint(this).Position;
                ell.Margin = new Thickness { Left = p.X - ell.Width / 2, Top = p.Y - ell.Width / 2 };
                ell.Tag = new Point(ell.Margin.Left - p.X, ell.Margin.Top - p.Y);
                ds.Add(e.Device, ell);
            };
            holdAndWait = (s, e) => {
                var ell = s as Ellipse;
                if (ell.Tag is DateTime) {
                    var s1 = (ell.Fill as RadialGradientBrush).GradientStops[1];
                    s1.Offset = Math.Max(0, s1.Offset - 0.07);
                    if (s1.Offset == 0) {
                        startToMoveKey(e);
                    }
                }
            };
            removeOrRestart = (s, e) => {
                var ell = s as Ellipse;
                if (ell.Tag is DateTime) {
                    var s1 = (ell.Fill as RadialGradientBrush).GradientStops[1];
                    if ((DateTime.Now - (DateTime)ell.Tag).Milliseconds < 555) {
                        keysPanel.Children.Remove(ell);
                    } else {
                        if (s1.Offset != 0) {
                            s1.Offset = 1;
                        }
                    }
                    ell.Tag = null;
                    return;
                }
            };
            tryToMoveKey = (s, e) => {
                var ell = s as Ellipse;
                if ((ell.Fill as RadialGradientBrush).GradientStops[1].Offset == 1) {
                    ell.Tag = DateTime.Now;
                    return;
                }
                if (ds.ContainsKey(e.Device)) {
                    return;
                }
                startToMoveKey(e);
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
                    sr.Offset = Math.Min(1, sr.Offset + 0.07);
                }
                ell.Margin = new Thickness { Left = t.X + p.X, Top = t.Y + p.Y };
            };
            confirmKey = (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var ell = ds[e.Device];
                ds.Remove(e.Device);
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
                if ((center - p).Length < settings.Width / 2) {
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
                ell.TouchDown += tryToMoveKey;
                ell.TouchLeave += removeOrRestart;
                ell.TouchMove += holdAndWait;
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
            stopChange = (s, e) => {
                if (e.Device != settingsDevice) {
                    return;
                }
                lost(null, null);
            };
            changeSize = (s, e) => {
                if (settingsDevice != e.Device) {
                    return;
                }
                var p = e.GetTouchPoint(this).Position;
                var v = p - center;
                settingsOutter.Opacity = Math.Min(1, v.Length / settings.Width);
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
                if (w < 0xf) {
                    w = 0xf;
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
                if (settingsDevice == null) {
                    return;
                }
                settingsDevice = null;
                settingsOutter.Opacity = 0;
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
                if (settingsDevice != null) {
                    return;
                }
                settingsDevice = e.Device;
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
    }
}