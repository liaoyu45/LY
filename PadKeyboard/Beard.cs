using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PadKeyboard {
    static class Beard {
        public static readonly Window Board = new Window {
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Top = 0,
            Left = 0,
            Width = SystemParameters.WorkArea.Width,
            Height = SystemParameters.WorkArea.Height,
            ResizeMode = ResizeMode.NoResize,
        };

        public static readonly double Height = Board.Height;
        public static readonly double Width = Board.Width;
        public const int KeysCountDefault = 20;
        public const int KeysCountMax = 104;
        public static readonly double MaxRadius = Width / 12;

        public static int KeysCount = 20;
        public static double Radius = Width / 24;

        public static IEnumerable<Point> Points;


        [STAThread()]
        public static void Main() {
            var Queue = new Gods.Steps.StepQueue();
            Queue.Steps.Add(new Step1SetRadiusAndCount());
            Queue.Steps.Add(new Step2AddPoints());
            Queue.Start();
            var ps = new Dictionary<InputDevice, List<Point>>();
            var fs = 0;
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

        public static Point GetPoint(this TouchEventArgs e) {
            var p = e.GetTouchPoint(Board).Position;
            p.X += Board.Left;
            p.Y += Board.Top;
            p.Y = Math.Max(Math.Min(Height - Radius, p.Y), Radius);
            p.X = Math.Max(Math.Min(Width - Radius, p.X), Radius);
            return p;
        }
    }
}
