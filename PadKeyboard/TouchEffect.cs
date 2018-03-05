using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class TouchEffect {
        private readonly Point center;

        public TouchEffect(Grid self, Trace t, double minRange) {
            center = t.Center;
            InputDevice d = null;
            MainBall = new Ellipse {
                Fill = DefaultBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = t.Radius * 2,
                Width = t.Radius * 2,
                Margin = new Thickness { Left = t.Center.X - t.Radius, Top = t.Center.Y - t.Radius }
            };
            Ball = new Ellipse {
                Fill = DefaultBrush,
                Width = t.Radius,
                Height = t.Radius,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Line = new Line {
                StrokeThickness = t.Radius / 11,
                HorizontalAlignment = HorizontalAlignment.Left,
                Stroke = DefaultBrush,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false
            };
            EventHandler<TouchEventArgs> start = (s, e) => {
                if (Disabled || d != null) {
                    return;
                }
                d = e.Device;
                MoveTo(e.GetTouchPoint(self).Position);
                t.Released = false;
                var sc = RandomColor();
                Ball.Fill = MainBall.Fill = new RadialGradientBrush(sc);
                Line.Stroke = new LinearGradientBrush(sc);
                ReadyToMove?.Invoke(t.Clone());
            };
            MainBall.TouchDown += start;
            Ball.TouchDown += start;
            Line.X1 = t.Center.X;
            Line.Y1 = t.Center.Y;
            MoveTo(t.Center);
            self.Children.Add(Line);
            self.Children.Add(Ball);
            self.Children.Add(MainBall);
            Func<TouchEventArgs, bool> tryMove = e => {
                if (e.Device != d) {
                    return false;
                }
                var to = e.GetTouchPoint(self).Position;
                t.Direction = to - t.Center;
                MoveTo(to);
                return true;
            };
            self.TouchMove += (s, e) => {
                if (tryMove(e)) {
                    Moving?.Invoke(t.Clone());
                }
            };
            self.TouchLeave += (s, e) => {
                if (!tryMove(e)) {
                    return;
                }
                d = null;
                t.Released = true;
                if (t.Direction.Length < minRange) {
                    t.Direction = new Vector();
                    Reset();
                } else {
                    StopMoving?.Invoke(t.Clone());
                }
            };
        }

        public void MoveTo(Point to) {
            Ball.Margin = new Thickness { Left = to.X - Ball.Width / 2, Top = to.Y - Ball.Width / 2 };
            Line.X2 = to.X;
            Line.Y2 = to.Y;
        }

        public void Reset() {
            MoveTo(center);
            Ball.Fill = MainBall.Fill = Line.Stroke = DefaultBrush;
        }

        public Ellipse Ball { get; }
        public Line Line { get; }
        public Ellipse MainBall { get; }
        public RadialGradientBrush DefaultBrush { get; set; } = new RadialGradientBrush(Colors.Black, Colors.White);
        public bool Disabled { get; set; }

        public event Action<Trace> ReadyToMove, Moving, StopMoving;

        public static GradientStopCollection RandomColor(int count = 3) {
            var b = new byte[3];
            var r = new List<Color>();
            var random = new Random();
            for (var i = 0; i < count; i++) {
                random.NextBytes(b);
                r.Add(Color.FromArgb(byte.MaxValue, b[0], b[1], b[2]));
            }
            count--;
            return new GradientStopCollection(r.Select((c, i) => new GradientStop {
                Color = c,
                Offset = i / (double)count
            }));
        }
    }
}
