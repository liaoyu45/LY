using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    public partial class FREEEEEE : Window {
        private double radius;
        private int currentCount = 12376826;

        public FREEEEEE() {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            Background = new SolidColorBrush(new Color { A = 1 });
            AllowsTransparency = true;
            Top = 0;
            Left = 0;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            ResizeMode = ResizeMode.NoResize;
            
            radius = Math.Min(Width, Height) / 10;
            new Beard();
            settings();
        }

        private void settings() {
            var ds = new Dictionary<InputDevice, Grid>();
            Func<TouchEventArgs, Point> getPoint = e => {
                var p = e.GetTouchPoint(workingPanel).Position;
                p.Y = Math.Max(Math.Min(Height - radius, p.Y), radius);
                p.X = Math.Max(Math.Min(Width - radius, p.X), radius);
                return p;
            };
            workingPanel.TouchMove += (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var p = getPoint(e);
                ds[e.Device].Margin = new Thickness { Left = p.X - radius, Top = p.Y - radius };
            };
            workingPanel.TouchLeave += (s, e) => {
                if (ds.ContainsKey(e.Device)) {
                    ds[e.Device].Tag = e.GetTouchPoint(workingPanel).Position;
                    ds.Remove(e.Device);
                }
            };
            workingPanel.TouchDown += (s, e) => {
                if (keysPanel.Children.Count >= currentCount) {
                    return;
                }
                if (ds.ContainsKey(e.Device)) {//why
                    return;
                }
                var p = getPoint(e);
                foreach (var item in keysPanel.Children.Cast<Grid>()) {
                    if (((Point)item.Tag - p).Length < radius) {
                        if (!ds.ContainsValue(item)) {
                            ds.Add(e.Device, item);
                        }
                        return;
                    }
                }
                var grid = new Grid {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - radius, Top = p.Y - radius },
                    Width = radius * 2,
                    Height = radius * 2,
                    Tag = p
                };
                var ell = new Ellipse {
                    Width = radius * 2,
                    Height = radius * 2,
                    Fill = new SolidColorBrush(new Color { ScA = 1, ScR = .1f, ScG = .1f, ScB = .1f })
                };
                var txt = new TextBlock {
                    Text = "?",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                grid.Children.Add(ell);
                grid.Children.Add(txt);
                ds.Add(e.Device, grid);
                keysPanel.Children.Add(grid);
            };
        }
    }
}