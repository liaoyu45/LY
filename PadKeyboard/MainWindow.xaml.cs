using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {

    public partial class MainWindow : Window {
        private Brush defaultColor;
        private Brush hoverColor;

        public MainWindow() {
            InitializeComponent();
            self = this;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            var r = .6180339887498948482045868343656;
            keysPanel.Height = Width * r * (4 + r) / (11 + r);
            infoPanel.Height = Height - keysPanel.Height;
            KeyDown += (s, e) => {
                if (e.Key == Key.Escape) {
                    Close();
                }
            };
            defaultColor = Resources["defaultColor"] as Brush;
            hoverColor = Resources["hoverColor"] as Brush;
            Loaded += delegate {
                //Worker.target = new WindowInteropHelper(this).Handle;
                EventHandler<TouchEventArgs> enter = (s, e) => {
                    (s as Grid).Background = hoverColor;
                };
                EventHandler<TouchEventArgs> leave = (s, e) => {
                    (s as Grid).Background = defaultColor;
                };
                foreach (var item in keysPanel.Children.OfType<Grid>()) {
                    var n = item.Name;
                    item.TouchEnter += enter;
                    item.TouchLeave += leave;
                    item.Background = defaultColor;
                    Worker.rects.Add(n, new Rect(item.PointToScreen(new Point()), item.RenderSize));
                    if (item.Children.Count > 0) {
                        continue;
                    }
                    var b = new TextBlock();
                    b.HorizontalAlignment = HorizontalAlignment.Center;
                    b.VerticalAlignment = VerticalAlignment.Center;
                    b.Text = n;
                    item.Children.Add(b);
                }
            };
            addPathRecords();
            addPickerEvents();
            addKeysEvents();
        }

        private void addPathRecords() {
            var gs = new Dictionary<InputDevice, Grid>();
            Func<double[], Line> getLine = rect => new Line {
                Stroke = defaultColor,
                StrokeThickness = 4,
                X1 = rect[0],
                Y1 = rect[1],
                X2 = rect[2],
                Y2 = rect[3]
            };
            keysPanel.TouchDown += (s, e) => {
                var p = e.GetTouchPoint(this).Position;
                var g = new Grid();
                g.VerticalAlignment = VerticalAlignment.Stretch;
                g.HorizontalAlignment = HorizontalAlignment.Stretch;
                g.Children.Add(getLine(new[] { p.X, p.Y, p.X, p.Y }));
                path.Children.Add(g);
                gs.Add(e.Device, g);
            };
            TouchMove += (s, e) => {
                if (!gs.ContainsKey(e.Device)) {
                    return;
                }
                var p = e.GetTouchPoint(this).Position;
                var item = gs[e.Device];
                var last = item.Children[item.Children.Count - 1] as Line;
                item.Children.Add(getLine(new[] { last.X2, last.Y2, p.X, p.Y }));
            };
            LostFocus += delegate { path.Children.RemoveRange(0, path.Children.Count); ; };
            TouchLeave += (s, e) => {
                if (!gs.ContainsKey(e.Device)) {
                    return;
                }
                path.Children.Remove(gs[e.Device]);
                gs.Remove(e.Device);
            };
        }

        private void addPickerEvents() {
            bool moving = false;
            TouchUp += (s, e) => {
                moving = false;
            };
            TouchMove += (s, e) => {
                if (!moving) {
                    return;
                }
            };
            TouchDown += (s, e) => {
                moving = true;
            };
        }

        private void addKeysEvents() {
            var ds = Enumerable.Range(0, 10).ToDictionary(n => n, n => default(InputDevice));
            LostFocus += delegate {
                ds.Clear();
                Worker.removeAll();
            };
            keysPanel.TouchDown += (s, e) => {
                ds[Worker.addFinger(e.GetTouchPoint(this).Position)] = e.Device;
            };
            TouchMove += (s, e) => {
                var f = ds.FirstOrDefault(d => d.Value == e.Device);
                if (f.Value == null) {
                    return;
                }
                if (!Worker.moveFinger(f.Key, e.GetTouchPoint(this).Position)) {
                    ds.Remove(f.Key);
                }
            };
            TouchLeave += (s, e) => {
                foreach (var item in ds) {
                    if (item.Value == e.Device) {
                        ds.Remove(item.Key);
                        Worker.removeFinger(item.Key);
                        break;
                    }
                }
            };
        }

        public static MainWindow self;

    }
}