using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Step3Sort : KeyStep {

        private List<Point> orderedPoints = new List<Point>();

        private double r => Beard.Radius;
        public Step3Sort() {
            var indexInfo = new TextBlock { Text = "0/" + Beard.KeysCount, FontSize = .3, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            var indexBox = ShadowBox.SquareButton(indexInfo);
            indexBox.Height = indexBox.Width = r * 2;
            Content.Children.Add(indexBox);
            Moveable(indexBox);
            var points = Elements.Visual<Ellipse>(Beard.RawPoints, r);
            Elements.AddClick(indexBox, delegate {
                indexInfo.Text = "0/" + points.Count();
                orderedPoints.Clear();
                foreach (var item in points) {
                    item.Fill = Brushes.pending;
                }
                for (int i = Content.Children.Count - 1; i >= 0; i--) {
                    var c = Content.Children[i];
                    if (c is Grid && c != indexBox) {
                        Content.Children.RemoveAt(i);
                    }
                    if (c is Ellipse) {
                        (c as Ellipse).Fill = Brushes.pending;
                    }
                }
            }, null);
            Action moveIndexInfo = () => {
                Content.Children.Remove(indexBox);
                Content.Children.Add(indexBox);
            };
            EventHandler<TouchEventArgs> addTextTag = (s, e) => {
                var ell = s as Ellipse;
                var p = (Point)ell.Tag;
                if (orderedPoints.Contains(p)) {
                    return;
                }
                orderedPoints.Add(p);
                ell.Fill = Brushes.picked;
                var index = orderedPoints.Count + string.Empty;
                indexInfo.Text = index + '/' + points.Count();
                var g = Elements.Visual<Grid>(p, r);
                g.Children.Add(new TextBlock {
                    Text = index,
                    FontSize = Beard.Radius,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
                Content.Children.Add(g);
                moveIndexInfo();
            };
            foreach (var item in points) {
                Content.Children.Add(item);
                item.Fill = Brushes.pending;
                item.TouchDown += addTextTag;
            }
            moveIndexInfo();
        }

        protected override int Finish() {
            var ok = orderedPoints.Count == Beard.KeysCount;
            if (ok) {
                Beard.OrderedPoints = orderedPoints;
            }
            return ok ? 1 : 0;
        }
    }
}
