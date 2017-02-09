using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Step3Sort : Gods.Steps.Step {

        private Grid content = Elements.BgA1Grid();

        private List<Point> orderedPoints = new List<Point>();
        private TextBlock indexInfo = new TextBlock {
            FontSize = Beard.Height / 32,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        private Grid indexBox;
        private IEnumerable<Ellipse> points;

        private double r => Beard.Radius;
        public Step3Sort() {
            indexBox = BoxShadow.Create(r * 4, r * 4 * .382, r / 2, r / 2, new GradientStop { Color = Colors.Black }, new GradientStop { Color = Colors.White, Offset = 1 });
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
                    item.Fill = Brushes.pending;
                }
                for (int i = content.Children.Count - 1; i >= 0; i--) {
                    var c = content.Children[i];
                    if (c is Grid && c != indexBox) {
                        content.Children.RemoveAt(i);
                    }
                    if (c is Ellipse) {
                        (c as Ellipse).Fill = Brushes.pending;
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
            var r = Beard.Radius;
            points = Elements.Visual<Ellipse>(Beard.RawPoints, r);
            indexInfo.Text = "0/" + points.Count();
            System.EventHandler<TouchEventArgs> addTextTag = (s, e)=> {
                var ell = s as Ellipse;
                var p = (Point)ell.Tag;
                if (orderedPoints.Contains(p)) {
                    return;
                }
                ell.Fill = Brushes.picked;
                var index = orderedPoints.Count + 1 + string.Empty;
                indexInfo.Text = index + '/' + points.Count();
                var g = Elements.Visual<Grid>(p, r);
                g.Children.Add(new TextBlock {
                    Text = index,
                    FontSize = Beard.Radius,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
                content.Children.Add(g);
                moveIndexInfo();
                orderedPoints.Add(p);
            };
            foreach (var item in points) {
                content.Children.Add(item);
                item.Fill = Brushes.pending;
                item.TouchDown += addTextTag;
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
