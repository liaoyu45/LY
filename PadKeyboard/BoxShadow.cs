using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    public class BoxShadow {
        public static Grid Create(double width, double height, double innerX, double innerY, params GradientStop[] gs) {
            var inner = new Rect { X = innerX, Y = innerY, Width = width - innerX * 2, Height = height - innerY * 2 };
            return Create(new Size(width, height), inner, gs);
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
