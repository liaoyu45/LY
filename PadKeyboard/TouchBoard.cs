using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class TouchBoard : Window {
        public bool Async { get; set; }

        Grid content = new Grid();
        public TouchBoard() {
            Background = new SolidColorBrush(new Color { A = 1 });
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
        }

        public void Layout(double radius, List<Point> traces) {
            content.Children.Clear();
            for (var i = 0; i < traces.Count; i++) {
                var item = traces[i];
                var eff = new TraceEffect {
                    Index = i,
                    Width = radius * 2,
                    Margin = new Thickness { Left = item.X - radius, Top = item.Y - radius },
                };
                content.Children.Add(eff);
            }
        }

        private void add(TraceEffect sender) {
            traces.Add(new Trace(sender));
            if (traces.GroupBy(i => i.Index).All(i => i.Last().Released)) {
                if (Async) {
                    Finished?.BeginInvoke(this, new TraceEventArgs { Traces = traces }, null, null);
                } else {
                    Finished?.Invoke(this, new TraceEventArgs { Traces = traces });
                }
                traces = new List<ITrace>();
            }
        }

        public bool WithEffect { get; set; }

        public event EventHandler<TraceEventArgs> Finished;

        public class TraceEventArgs : EventArgs {
            public List<ITrace> Traces { get; set; }
        }

        private List<ITrace> traces = new List<ITrace>();




        class TraceEffect : UserControl, ITrace {
            public int Index { get; set; }
            public Vector Direction { get; set; }
            public double Radius => Width / 2;
            public bool Released => d == null;

            private InputDevice d;
            private Point center;
            private TouchBoard parent => Parent as TouchBoard;

            public Line Line { get; private set; }
            public Ellipse Ball { get; private set; }
            public Ellipse MainBall => Content as Ellipse;

            public TraceEffect() {
                Content = new Ellipse {
                    Fill = Brushes.pending
                };
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
                MainBall.TouchDown += start;
            }

            protected override void OnVisualParentChanged(DependencyObject oldParent) {
                base.OnVisualParentChanged(oldParent);
                Height = Width;
                parent.TouchMove += (s, e) => {
                    if (d != e.Device) {
                        return;
                    }
                    move(e);
                };
                parent.TouchLeave += (s, e) => {
                    if (d != e.Device) {
                        return;
                    }
                    d = null;
                    move(e);
                    leaveEffect();
                    parent.add(this);
                };
            }

            private void move(TouchEventArgs e) {
                var to = e.GetTouchPoint(parent).Position;
                Direction = to - center;
                moveEffect(to);
            }

            private void start(object sender, TouchEventArgs e) {
                if (d != null) {
                    return;
                }
                d = e.Device;
                var p = e.GetTouchPoint(Parent as IInputElement).Position;
                if (parent.WithEffect) {
                    startEffect(p);
                } else {
                    center = p;
                    Direction = new Vector();
                }
                parent.add(this);
            }

            private void startEffect(Point p) {
                MainBall.Fill = Brushes.picked;
                Line.X2 = p.X;
                Line.Y2 = p.Y;
                Ball.Margin = new Thickness { Left = p.X - Ball.Width / 2, Top = p.Y - Ball.Width / 2 };
            }

            private void moveEffect(Point to) {
                if (parent.WithEffect) {
                    Ball.Margin = new Thickness { Left = to.X - Ball.Width / 2, Top = to.Y - Ball.Width / 2 };
                    Line.X2 = to.X;
                    Line.Y2 = to.Y;
                }
            }

            private void leaveEffect() {
                if (parent.WithEffect) {
                    Ball.Fill = Brushes.picked;
                    MainBall.Fill = Brushes.pending;
                }
            }

            private void LoadEffect() {
                if (!parent.WithEffect) {
                    return;
                }
                center = new Point(Margin.Left + Radius, Margin.Top + Radius);
                var to = center + Direction;
                Line = new Line {
                    IsHitTestVisible = false,
                    Stroke = Brushes.pending,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    X1 = center.X,
                    Y1 = center.Y,
                    X2 = to.X,
                    Y2 = to.Y
                };
                Ball = new Ellipse {
                    Fill = Brushes.pending,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = Width / 3,
                    Width = Width / 3,
                };
                Ball.TouchDown += start;
                parent.content.Children.Add(Line);
                parent.content.Children.Add(Ball);
                moveEffect(to);
            }

            public void ResetEffect() {
                if (!parent.WithEffect) {
                    return;
                }
                Line.Stroke = Brushes.pending;
                Line.X2 = Line.X1;
                Line.Y2 = Line.Y1;
                Ball.Fill = Brushes.pending;
                Direction = new Vector();
            }
        }
    }
}
