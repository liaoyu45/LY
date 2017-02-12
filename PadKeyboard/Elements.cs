using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PadKeyboard {
    class Elements {

        static GradientStop[] gs = new Dictionary<double, Color> {
                { 0, Colors.White },
                { .02, Colors.Black },
                { .20, Colors.White},
                { .24, Colors.Black},
                { .27, Colors.White }
            }.Select(item => new GradientStop { Offset = item.Key, Color = item.Value }).ToArray();
        public static Grid ShadowButton(Action<TextBlock> content = null) {
            var b = BoxShadow.Create(gs);
            var tb = new TextBlock {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            b.Children.Add(tb);
            content?.Invoke(tb);
            return b;
        }

        public static Grid CenterTextBlock() {
            var r = new Grid();
            r.Children.Add(new TextBlock {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            return r;
        }

        public static T Visual<T>(Point p, double r) where T : FrameworkElement, new() {
            return new T {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = r * 2,
                Height = r * 2,
                Margin = new Thickness { Left = p.X - r, Top = p.Y - r },
                Tag = p
            };
        }

        public static IEnumerable<T> Visual<T>(IEnumerable<Point> ps, double r) where T : FrameworkElement, new() {
            foreach (var item in ps) {
                yield return Visual<T>(item, r);
            }
        }

        public static Grid BgA1Grid(Action<Grid> dowith = null) {
            var t = BgA1(dowith);
            return t;
        }

        public static T BgA1<T>(Action<T> dowith = null) where T : new() {
            var t = new T();
            try {
                typeof(T).GetProperty("Background").SetValue(t, new SolidColorBrush(new Color { A = 1 }));
            } catch {
            }
            dowith?.Invoke(t);
            return t;
        }
    }
}
