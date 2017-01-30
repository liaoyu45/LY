using Gods.Steps;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    internal class Step2AddPoints : Step {

        private Grid content = new Grid();
        private Grid addPanel = new Grid { Background = new SolidColorBrush { Color = new Color { A = 1 } } };
        private Grid effectPanel = new Grid();

        public Step2AddPoints() {
            var ds = new Dictionary<InputDevice, Ellipse>();
            addPanel.TouchMove += (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var p = e.GetPoint();
                ds[e.Device].Margin = new Thickness { Left = p.X - Beard.Radius, Top = p.Y - Beard.Radius };
            };
            addPanel.TouchLeave += (s, e) => {
                if (ds.ContainsKey(e.Device)) {
                    var ell = ds[e.Device];
                    var point = new Point(ell.Margin.Left + Beard.Radius, ell.Margin.Top + Beard.Radius);
                    foreach (var item in mapCenter()) {
                        if (item.Key == ell) {
                            continue;
                        }
                        if ((point - item.Value).Length < Beard.Radius * 2) {
                            effectPanel.Children.Remove(item.Key);
                        }
                    }
                    ds.Remove(e.Device);
                }
            };
            addPanel.TouchDown += (s, e) => {
                if (ds.ContainsKey(e.Device)) {//why
                    return;
                }
                var p = e.GetPoint();
                foreach (var item in mapCenter()) {
                    if ((item.Value - p).Length < Beard.Radius) {
                        if (!ds.ContainsValue(item.Key)) {
                            ds.Add(e.Device, item.Key);
                        }
                        return;
                    }
                }
                if (effectPanel.Children.Count >= Beard.KeysCount) {
                    effectPanel.Children.RemoveRange(Beard.KeysCount, 2198714);
                    Beard.Points = mapCenter().Values;
                    return;
                }
                var grid = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - Beard.Radius, Top = p.Y - Beard.Radius },
                    Width = Beard.Radius * 2,
                    Height = Beard.Radius * 2,
                    Fill = new SolidColorBrush(new Color { A = 222 })
                };
                ds.Add(e.Device, grid);
                effectPanel.Children.Add(grid);
            };
            content.Children.Add(effectPanel);
            content.Children.Add(addPanel);
        }

        protected override bool Finish() {
            return effectPanel.Children.Count == Beard.KeysCount;
        }

        protected override void Init(int offset) {
            Beard.Board.Content = content;
            effectPanel.Children.Clear();
            addPanel.Children.Clear();
        }

        public Dictionary<Ellipse, Point> mapCenter() {
            return effectPanel.Children.Cast<Ellipse>().ToDictionary(e => e, e => new Point(e.Margin.Left + Beard.Radius, e.Margin.Top + Beard.Radius));
        }
    }
}