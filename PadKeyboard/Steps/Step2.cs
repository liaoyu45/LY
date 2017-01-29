using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard.Steps {
    internal class Step2 : Step {

        protected override int Index { get; set; } = 2;

        private int currentCount = 26;
        private Grid addPanel = new Grid { Background = new SolidColorBrush { Color = new Color { A = 1 } } };
        private Grid effectPanel = new Grid();

        protected override void Cancel() {
            effectPanel.Children.Clear();
            addPanel.Children.Clear();
            Target.content.Children.Clear();
        }

        protected override void Initiate(object result, bool direction) {
            var radius = (double)result;
            Target.content.Children.Add(effectPanel);
            Target.content.Children.Add(addPanel);
            var ds = new Dictionary<InputDevice, Grid>();
            var ps = new Dictionary<InputDevice, Point>();
            Func<TouchEventArgs, Point> getPoint = e => {
                var p = e.GetTouchPoint(addPanel).Position;
                p.Y = Math.Max(Math.Min(Target.Height - radius, p.Y), radius);
                p.X = Math.Max(Math.Min(Target.Width - radius, p.X), radius);
                return p;
            };
            addPanel.TouchMove += (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var p = getPoint(e);
                ps[e.Device] = p;
                ds[e.Device].Margin = new Thickness { Left = p.X - radius, Top = p.Y - radius };
            };
            addPanel.TouchLeave += (s, e) => {
                if (ds.ContainsKey(e.Device)) {
                    ds[e.Device].Tag = e.GetTouchPoint(addPanel).Position;
                    ds.Remove(e.Device);
                }
                ps.Remove(e.Device);
            };
            addPanel.TouchDown += (s, e) => {
                if (ds.ContainsKey(e.Device)) {//why
                    return;
                }
                var p = getPoint(e);
                foreach (var item in effectPanel.Children.Cast<Grid>()) {
                    if (((Point)item.Tag - p).Length < radius) {
                        if (!ds.ContainsValue(item)) {
                            ds.Add(e.Device, item);
                        }
                        return;
                    }
                }
                if (effectPanel.Children.Count >= currentCount) {
                    return;
                }
                var grid = new Grid {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - radius, Top = p.Y - radius },
                    Width = radius * 2,
                    Height = radius * 2,
                    Tag = p
                };
                grid.Children.Add(new Ellipse {
                    Width = radius * 2,
                    Height = radius * 2,
                    Fill = new SolidColorBrush(new Color { ScA = 1, ScR = .1f, ScG = .1f, ScB = .1f })
                });
                grid.Children.Add(new TextBlock {
                    Text = "?",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                ds.Add(e.Device, grid);
                effectPanel.Children.Add(grid);
            };
        }
    }
}