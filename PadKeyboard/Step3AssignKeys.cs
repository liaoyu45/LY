using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Step3AssignKeys : Gods.Steps.Step {

        private Grid content = Beard.BgA1Grid();

        private List<Point> orderedPoints = new List<Point>();
        private TextBlock indexInfo = new TextBlock {
            FontSize = Beard.Height / 32,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        private Grid indexBox;

        private readonly Brush picked = new RadialGradientBrush(Colors.White, Colors.Black);
        private readonly Brush pending = new RadialGradientBrush(Colors.Black, Colors.White);

        private IEnumerable<Ellipse> points;
        public Step3AssignKeys() {
            var w6 = Beard.Width / 6;
            var h22 = Beard.Height / 22;
            indexBox = BoxShadow.Create(w6, w6, h22, h22, new GradientStop { Color = Colors.Black }, new GradientStop { Color = Colors.White, Offset = 1 });
            indexBox.VerticalAlignment = VerticalAlignment.Top;
            indexBox.HorizontalAlignment = HorizontalAlignment.Left;
            indexBox.Background = new SolidColorBrush(Colors.White);
            indexBox.Children.Add(indexInfo);
            content.Children.Add(indexBox);
            InputDevice d = null;
            indexBox.TouchDown += (s, e) => {
                if (d == null) {
                    d = e.Device;
                }
            };
            content.TouchLeave += (s, e) => {
                if (e.Device == d) {
                    d = null;
                }
            };
            content.TouchMove += (s, e) => {
                if (d == null) {
                    return;
                }
                if (d == e.Device) {
                    var p = e.MoveInside(indexBox);
                    indexBox.Margin = new Thickness { Left = p.X, Top = p.Y };
                    return;
                }
                orderedPoints.Clear();
                indexInfo.Text = "0/" + points.Count();
                foreach (var item in points) {
                    item.Fill = pending;
                }
                for (int i = content.Children.Count - 1; i >= 0; i--) {
                    var c = content.Children[i];
                    if (c is Grid && c != indexBox) {
                        content.Children.RemoveAt(i);
                    }
                    if (c is Ellipse) {
                        (c as Ellipse).Fill = pending;
                    }
                }
            };
        }

        private void moveIndexInfo() {
            content.Children.Remove(indexBox);
            content.Children.Add(indexBox);
        }

        protected override void Init(int offset) {
            Beard.Content = content;
            content.Children.Clear();
            orderedPoints.Clear();
            points = Beard.Visual<Ellipse>(Beard.RawPoints);
            indexInfo.Text = "0/" + points.Count();
            foreach (var item in points) {
                content.Children.Add(item);
                item.Fill = pending;
                item.TouchDown += (s, e) => {
                    var ell = s as Ellipse;
                    var p = (Point)ell.Tag;
                    if (orderedPoints.Contains(p)) {
                        return;
                    }
                    ell.Fill = picked;
                    var index = orderedPoints.Count + 1 + string.Empty;
                    indexInfo.Text = index + '/' + points.Count();
                    var g = Beard.Visual<Grid>(p);
                    var id = new TextBlock {
                        Text = index,
                        FontSize = Beard.Radius,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    g.Children.Add(id);
                    content.Children.Add(g);
                    moveIndexInfo();
                    orderedPoints.Add(p);
                };
            }
            moveIndexInfo();
        }

        protected override bool Finish() {
            var ok = orderedPoints.Count == Beard.KeysCount;
            if (ok) {
                Beard.OrderedPoints = orderedPoints;
            }
            return ok;
        }

        protected override bool Cancel() {
            var ok = content.Children.OfType<Grid>().Count() == 1;
            if (ok) {
                Beard.OrderedPoints = null;
            }
            return ok;
        }
    }
}
