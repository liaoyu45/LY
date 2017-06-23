using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    public class ShadowBox {
        public static void ShowCorner(Grid r, int index, Action<VisualBrush> dowith = null) {
            var brush = r.Background as VisualBrush;
            var c = (brush?.Visual as Grid)?.Children;
            if (c == null) {
                return;
            }
            if (index > -1 && index < 4) {
                for (var i = 0; i < 4; i++) {
                    c[i].Visibility = Visibility.Hidden;
                }
                c[index].Visibility = Visibility.Visible;
            }
            dowith?.Invoke(brush);
        }


        static GradientStop[] gs = new Dictionary<double, Color> {
                { 0, Colors.White },
                { .02, Colors.Black },
                { .20, Colors.White},
                { .24, Colors.Black},
                { .27, Colors.White }
            }.Select(item => new GradientStop { Offset = item.Key, Color = item.Value }).ToArray();

        public static Grid SquareButton(UIElement content = null) {
            var b = Create(gs);
            if (content != null) {
                ((b.Background as VisualBrush).Visual as Grid).Children.Add(content);
            }
            return b;
        }


        public static Grid Create() {
            return Create(new[] { new GradientStop(), new GradientStop(Colors.Black, 1) });
        }

        public static Grid Create(params GradientStop[] gs) {
            return new Grid {
                Background = new VisualBrush { Visual = Create(2, 2, 1, 1, gs) }
            };
        }

        public static Grid Create(double width, double height, double innerX, double innerY, params GradientStop[] gs) {
            return Create(new Size(width, height), new Rect { X = innerX, Y = innerY, Width = width - innerX * 2, Height = height - innerY * 2 }, gs);
        }

        public static Grid Create(Size size, Rect inner, params GradientStop[] gs) {
            Func<PointCollection> ps = () => new PointCollection(Enumerable.Range(0, 5).Select(i => new Point()));
            var coll = new GradientStopCollection(gs);
            var left = new Polygon();
            left.Points = ps();
            left.Points[1] = inner.TopLeft;
            left.Points[2] = inner.BottomLeft;
            left.Points[3] = new Point(0, size.Height);
            left.Fill = new LinearGradientBrush(coll, new Point(0, 0), new Point(1, 0));
            var top = new Polygon();
            top.Points = ps();
            top.Points[0] = new Point(size.Width, 0);
            top.Points[1] = inner.TopRight;
            top.Points[2] = inner.TopLeft;
            top.Points[4] = new Point(size.Width, 0);
            top.Fill = new LinearGradientBrush(coll, new Point(0, 0), new Point(0, 1));
            var right = new Polygon();
            right.Points = ps();
            right.Points[0] = new Point(size.Width, size.Height);
            right.Points[1] = inner.BottomRight;
            right.Points[2] = inner.TopRight;
            right.Points[3] = new Point(size.Width, 0);
            right.Points[4] = new Point(size.Width, size.Height);
            right.Fill = new LinearGradientBrush(coll, new Point(1, 0), new Point(0, 0));
            var btm = new Polygon();
            btm.Points = ps();
            btm.Points[0] = new Point(0, size.Height);
            btm.Points[1] = inner.BottomLeft;
            btm.Points[2] = inner.BottomRight;
            btm.Points[3] = new Point(size.Width, size.Height);
            btm.Points[4] = new Point(0, size.Height);
            btm.Fill = new LinearGradientBrush(coll, new Point(0, 1), new Point(0, 0));

            var box = new Grid { Width = size.Width, Height = size.Height };
            box.Children.Add(left);
            box.Children.Add(top);
            box.Children.Add(right);
            box.Children.Add(btm);
            return box;
        }
    }
}
