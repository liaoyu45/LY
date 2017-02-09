using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Math;

namespace PadKeyboard {
    static class Beard {
        private static readonly Window Board = new Window {
            Top = 0,
            Left = 0,
            Width = SystemParameters.WorkArea.Width,
            Height = SystemParameters.WorkArea.Height,//TODO: should config these value: top left width height.
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
            var touched = new Dictionary<InputDevice, List<Point>>();
            var touching = 0;
            Board.Loaded += delegate {
                //Queue.Add<Step0LoadLayout>();
                Queue.Add<Step1SetRadiusAndCount>();
                Queue.Add<Step2AddPoints>();
                Queue.Add<Step3Sort>();
                Queue.Add<Step4AssignKeys>();
                Queue.Start();
            };
            Board.KeyDown += (s, e) => {
                if (e.Key == Key.Escape) {
                    Board.Close();
                }
            };
            Board.TouchDown += (s, e) => {
                if (touched.Count > 1) {
                    touched.Clear();
                    touching = 0;
                    return;
                }
                touched[e.Device] = touched.Count == 0 ? null : new List<Point> { e.GetTouchPoint(Board).Position };
                touching++;
            };
            Board.TouchMove += (s, e) => {
                if (touched.ContainsKey(e.Device)) {
                    touched[e.Device]?.Add(e.GetTouchPoint(Board).Position);
                }
            };
            Board.TouchLeave += (s, e) => {
                touching--;
                if (touching < 1 && touched.Count == 2) {
                    var path = touched.Last().Value;
                    var vMax = path.Last() - path.First();
                    if (vMax.Length > Board.Width / 2) {
                        Queue.Move(vMax.X > 0 ? 1 : -1);
                    }
                }
                if (touching < 1) {
                    touched.Clear();
                    touching = 0;
                }
            };
            new Application().Run(Board);
        }

        public static Point MoveInside(this TouchEventArgs self, FrameworkElement e) {
            var p = self.GetTouchPoint(Board).Position;
            return new Point(Max(0, Min(Width - e.Width, p.X - e.Width / 2)), Max(0, Min(Height - e.Height, p.Y - e.Height / 2)));
        }
    }
}
