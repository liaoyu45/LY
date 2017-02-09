using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PadKeyboard {
    class Elements {

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
            var g = new Grid {
                Background = new SolidColorBrush(new Color { A = 1 }),
            };
            dowith?.Invoke(g);
            return g;
        }
    }
}
