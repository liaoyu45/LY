using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Math;

namespace PadKeyboard {
    class PieRadius {
        public static Ellipse Create(int r, int c, Dictionary<double, Color> gc) {
            var gs = new GradientStopCollection(gc.Select(p => new GradientStop(p.Value, p.Key)));
            var points = new List<Point>();
            for (var i = 0; i < c; i++) {
                var a0 = PI * 2 * i / c;
                var a1 = PI * 2 * (i + 1) / c;
                points.Add(new Point(1 + 1 * Cos(a1), 1 + 1 * Sin(a1)));
            }
            var center = new Point(1, 1);
            var size = new Size(1, 1);
            var g = new Grid();
            g.MaxHeight = g.MaxWidth = g.Height = g.Width = 1 * 2;
            g.Children.Add(new Ellipse { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Fill = new RadialGradientBrush(gs) });
            Action<Point, Point> add = (p0, p1) => {
                g.Children.Add(new Path {
                    Stroke = new SolidColorBrush(Colors.DarkBlue),
                    StrokeThickness = .003,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Data = new PathGeometry(new[]{new PathFigure(center, new PathSegment[]{
                        new LineSegment { Point = p0, IsStroked  = true },
                        new ArcSegment { Point = p1, Size = size, SweepDirection = SweepDirection.Clockwise ,IsStroked  = true} }, true) })
                });
            };
            for (var i = 0; i < points.Count - 1; i++) {
                var p0 = points[i];
                var p1 = points[i + 1];
                add(p0, p1);
            }
            add(points[points.Count - 1], points[0]);
            var ell = new Ellipse {
                Width = r,
                Height = r,
                Fill = new VisualBrush { Visual = g },
            };
            return ell;
        }
    }
}
