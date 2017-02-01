using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PadKeyboard {
    static class Beard {
        private static readonly Window Board = new Window {
            Top = 0,//TODO: should config this value.
            Left = 0,//TODO: should config this value.
            Width = SystemParameters.WorkArea.Width,//TODO: should config this value.
            Height = SystemParameters.WorkArea.Height,//TODO: should config this value.
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = new SolidColorBrush(),
            ResizeMode = ResizeMode.NoResize,
        };

        public static Grid Content {
            set { Board.Content = value; }
            get { return Board.Content as Grid; }
        }

        public static readonly double Height = Board.Height;
        public static readonly double Width = Board.Width;
        public const int KeysMin = 16;
        public const int KeysMax = 64;
        public const int KeysRange = 48;
        public static readonly double MaxRadius = Width / 12;

        public static int KeysCount = KeysMin;
        public static double Radius = Width / 24;

        public static IEnumerable<Point> RawPoints;
        public static IEnumerable<Point> OrderedPoints;

        [STAThread()]
        public static void Main() {
            var Queue = new Gods.Steps.StepQueue();
            var ps = new Dictionary<InputDevice, List<Point>>();
            var fs = 0;
            Board.Loaded += delegate {
                Queue.Steps.Add(new Step1SetRadiusAndCount());
                Queue.Steps.Add(new Step2AddPoints());
                Queue.Steps.Add(new Step3AssignKeys());
                Queue.Steps.Add(new Step4AssignKeys());
                Queue.Start();
            };
            Board.KeyDown += (s, e) => {
                if (e.Key == Key.Escape) {
                    Board.Close();
                }
            };
            Board.TouchDown += (s, e) => {
                if (ps.Count == 3) {
                    return;
                }
                ps[e.Device] = new List<Point> { e.GetTouchPoint(Board).Position };
                fs++;
            };
            Board.TouchMove += (s, e) => {
                if (!ps.ContainsKey(e.Device)) {
                    return;
                }
                ps[e.Device].Add(e.GetTouchPoint(Board).Position);
            };
            Board.TouchLeave += (s, e) => {
                fs--;
                if (fs < 1 && ps.Count == 3) {
                    var last = ps.Last().Value;
                    if (last.Count < 3) {
                        goto end;
                    }
                    var vMax = last[last.Count - 1] - last[0];
                    if (Math.Abs(vMax.X) < Board.Width / 2) {
                        goto end;
                    }
                    if (vMax.Length > Board.Width / 2) {
                        Queue.Move(vMax.X > 0 ? 1 : -1);
                    }
                }
                end:
                if (fs < 1) {
                    ps.Clear();
                    fs = 0;
                }
            };
            new Application().Run(Board);
        }

        public static T Visual<T>(Point p) where T : FrameworkElement, new() {
            return new T {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = Radius * 2,
                Height = Radius * 2,
                Margin = new Thickness { Left = p.X - Radius, Top = p.Y - Radius },
                Tag = p
            };
        }

        public static IEnumerable<T> Visual<T>(IEnumerable<Point> ps) where T : FrameworkElement, new() {
            foreach (var item in ps) {
                yield return Visual<T>(item);
            }
        }

        public static Grid BgA1Grid(Action<Grid> dowith = null) {
            var g = new Grid {
                Background = new SolidColorBrush(new Color { A = 1 }),
            };
            dowith?.Invoke(g);
            return g;
        }

        public static Point MoveInside(this TouchEventArgs e, FrameworkElement ele) {
            var p = e.GetTouchPoint(Board).Position;
            return new Point(Math.Max(0, Math.Min(Width - ele.Width, p.X - ele.Width / 2)), Math.Max(0, Math.Min(Height - ele.Height, p.Y - ele.Height / 2)));
        }
    }
}
