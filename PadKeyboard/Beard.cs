using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Beard {
        internal static void fillKeys(Grid keysPanel) {
            var radius = 0d;
            var currentCount = 0;
            var ds = new Dictionary<InputDevice, Grid>();
            var workingPanel = new Grid() {Width = keysPanel.Width,  };
            Func<TouchEventArgs, Point> getPoint = e => {
                var p = e.GetTouchPoint(workingPanel).Position;
                p.Y = Math.Max(Math.Min(SystemParameters.WorkArea. Height - radius, p.Y), radius);
                p.X = Math.Max(Math.Min(SystemParameters.WorkArea.Width - radius, p.X), radius);
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
