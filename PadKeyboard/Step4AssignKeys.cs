using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Math;

namespace PadKeyboard {
    class Step4AssignKeys : Gods.Steps.Step {
        private Grid content = Elements.BgA1Grid();
        private Ellipse current;
        private Grid keyPanel = Elements.BgA1Grid();

        private double r => Beard.Radius;
        UIElementCollection ps;
        private Dicks ds;

        public Step4AssignKeys() {
            ds = new Dicks(r);
            var ddddd = new List<InputDevice>();
            var createEffect = new Func<Grid>(() => {
                var e = BoxShadow.Create(new GradientStop { Color = Colors.Black }, new GradientStop { Offset = 1 });
                e.Width = e.Height = r * 4;
                e.HorizontalAlignment = HorizontalAlignment.Left;
                e.VerticalAlignment = VerticalAlignment.Top;
                ps = ((e.Background as VisualBrush).Visual as Grid).Children;
                for (var i = 0; i < ps.Count; i++) {
                    ps[i].Visibility = Visibility.Hidden;
                }
                return e;
            });
            var editting = new Dictionary<Ellipse, Grid>();
            content.TouchLeave += (s, e) => {
                if (!ddddd.Contains(e.Device)) {
                    return;
                }

                if (editting.Count == 0) {
                    Beard.Queue.Move(1);

                }
            };
            var gest = Gests.Click;
            content.TouchMove += (s, e) => {
                if (!ddddd.Contains(e.Device)) {
                    return;
                }
                var pc = (Point)current.Tag;
                int newOne;
                var v = e.GetTouchPoint(content).Position - pc;
                if (v.Length < r / Sqrt(2)) {
                    newOne = -1;
                } else if (Abs(v.Y / v.X) < 1) {
                    newOne = v.X < 0 ? 0 : 2;
                } else {
                    newOne = v.Y < 0 ? 1 : 3;
                }
                var oldOne = (int)gest;
                if (newOne != oldOne) {
                    if (oldOne > -1 && oldOne < ps.Count) {
                        ps[oldOne].Visibility = Visibility.Hidden;
                    }
                    gest = (Gests)newOne;
                }
                if (newOne > -1) {
                    if (newOne < ps.Count) {
                        ps[newOne].Visibility = Visibility.Visible;
                    }
                    var len = Min(v.Length, r * 2);
                    var effect = createEffect();
                    effect.Width = effect.Height = len * 2;
                    effect.Margin = new Thickness { Left = pc.X - len, Top = pc.Y - len };
                    content.Children.Add(effect);
                    editting.Add(s as Ellipse, effect);
                }
            };
        }

        protected override void Init(int offset) {
            Beard.Content = content;
            if (offset < 0) {
                return;
            }
            content.Children.Clear();
            EventHandler<TouchEventArgs> editting = (s, e) => {
                d = e.Device;
                current = s as Ellipse;
                current.Fill = Brushes.picked;
            };
            var points = Elements.Visual<Ellipse>(Beard.OrderedPoints, r);
            foreach (var item in points) {
                item.Fill = Brushes.pending;
                content.Children.Add(item);
                item.TouchDown += editting;
            }
        }
    }
}
