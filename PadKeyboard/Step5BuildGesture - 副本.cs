using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Step5BuildGesture : KeyStep {
        public Step5BuildGesture() {
            for (var i = 0; i < Beard.OrderedPoints.Count; i++) {
                var item = Beard.OrderedPoints[i];
                Content.MoveEffect(new Trace { Center = item, Index = i }, Beard.Radius / 3 * 2);
            }
        }

        private void add(TraceEffect sender) {
            traces.Add(new Trace(sender));
            if (traces.GroupBy(i => i.Index).All(i => i.Last().Released)) {
                Task.Run(() => Finished?.Invoke(this, new TraceEventArgs { Traces = traces }));
                traces.Clear();
            }
        }

        public event EventHandler<TraceEventArgs> Finished;

        public class TraceEventArgs : EventArgs {
            public List<ITrace> Traces { get; set; }
        }

        private List<ITrace> traces = new List<ITrace>();

        class TraceEffect : ITrace {
            public int Index { get; set; }
            public Vector Direction { get; set; }
            public double Radius { get; }
            public bool Released { get; set; }
            public Point Center { get; private set; }


            public Ellipse MainBall { get; } = new Ellipse {
                Fill = Brushes.pending,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };


            public TraceEffect(Grid parent, double radius, Point center, Vector direction, double minLength, int i) {
                Index = i;
                Center = center;
                Radius = radius;
                Direction = direction;
                MainBall.Height = MainBall.Width = radius * 2;
                MainBall.Margin = new Thickness { Left = center.X - radius, Top = center.Y - radius };
                InputDevice d = null;
                var ball = new Ellipse {
                    Fill = Brushes.pending,
                    Width = radius,
                    Height = radius,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                var line = new Line {
                    Stroke = Brushes.pending,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    IsHitTestVisible = false
                };
                Action<Point> moveTo = newTo => {
                    ball.Margin = new Thickness { Left = newTo.X - ball.Width / 2, Top = newTo.Y - ball.Width / 2 };
                    line.X2 = newTo.X;
                    line.Y2 = newTo.Y;
                };
                EventHandler<TouchEventArgs> start = (s, e) => {
                    if (d != null) {
                        return;
                    }
                    d = e.Device;
                    MainBall.Fill = Brushes.picked;
                    moveTo(e.GetTouchPoint(parent).Position);
                    ReadyToMove?.Invoke(this, EventArgs.Empty);
                };
                MainBall.TouchDown += start;
                ball.TouchDown += start;
                var to = center + Direction;
                line.X1 = center.X;
                line.Y1 = center.Y;
                moveTo(to);
                ball.Height = Width / 3;
                ball.Width = Width / 3;
                parent.Children.Add(line);
                parent.Children.Add(MainBall);
                parent.Children.Add(ball);
                moveTo(to);
                Func<TouchEventArgs, bool> tryMove = e => {
                    if (e.Device != d) {
                        return false;
                    }
                    var newTo = e.GetTouchPoint(parent).Position;
                    Direction = newTo - Center;
                    moveTo(newTo);
                    return true;
                };
                parent.TouchMove += (s, e) => {
                    if (tryMove(e)) {
                        Moving?.Invoke(this, EventArgs.Empty);
                    }
                };
                Func<GradientStopCollection> tempSc = () => {
                    var r = new Random();
                    var b = new byte[3];
                    var rr = new List<Color>();
                    for (var ii = 0; ii < 3; ii++) {
                        r.NextBytes(b);
                        rr.Add(Color.FromArgb(255, b[0], b[1], b[2]));
                    }
                    var os = 0;
                    return new GradientStopCollection(rr.Select(rrrr => new GradientStop {
                        Color = rrrr,
                        Offset = os++ / 2.0
                    }));
                };
                parent.TouchLeave += (s, e) => {
                    if (!tryMove(e)) {
                        return;
                    }
                    d = null;
                    Released = true;
                    if (Direction.Length < minLength) {
                        Direction = new Vector();
                        moveTo(center);
                    } else {
                        var sc = tempSc();
                        ball.Fill = MainBall.Fill = new RadialGradientBrush(sc);
                        line.Fill = new LinearGradientBrush(sc);
                    }
                    Moved?.Invoke(this, EventArgs.Empty);
                };
            }

            public event EventHandler ReadyToMove;
            public event EventHandler Moving;
            public event EventHandler Moved;
        }
    }
}
