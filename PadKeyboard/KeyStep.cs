using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Math;

namespace PadKeyboard {
    abstract class KeyStep : Gods.Steps.Step {

        private static readonly Window board = new Window {
            Top = 0,
            Left = 0,
            Width = SystemParameters.WorkArea.Width,
            Height = SystemParameters.WorkArea.Height,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = new SolidColorBrush(),
            ResizeMode = ResizeMode.NoResize,
        };
        public static readonly double Height = board.Height;
        public static readonly double Width = board.Width;
        public static readonly double MaxRadius = Width / 12;

        public Grid Content { get; } = new Grid {
            Background = new SolidColorBrush(new Color { A = 1 }), Width = Width, Height = Height
        };

        private bool ever;
        protected override void Init(int offset) {
            if (ever && offset > 0) {
                WillRecreate = true;
                return;
            }
            ever = true;
            board.Content = Content;
        }

        protected static Point Inside(TouchEventArgs self, FrameworkElement e) {
            var p = self.GetTouchPoint(board).Position;
            return new Point(Max(0, Min(Width - e.Width, p.X - e.Width / 2)), Max(0, Min(Height - e.Height, p.Y - e.Height / 2)));
        }

        protected static void Moveable(FrameworkElement ele) {
            InputDevice d = null;
            ele.HorizontalAlignment = HorizontalAlignment.Left;
            ele.VerticalAlignment = VerticalAlignment.Top;
            ele.TouchDown += (s, e) => {
                if (d != e.Device) {
                    d = e.Device;
                }
            };
            board.TouchMove += (s, e) => {
                if (d != e.Device) {
                    return;
                }
                var p = e.GetTouchPoint(board).Position;
                ele.Margin = new Thickness {
                    Left = Max(0, Min(Width - ele.Width, p.X - ele.Width / 2)),
                    Top = Max(0, Min(Height - ele.Height, p.Y - ele.Height / 2))
                };
            };
            board.TouchLeave += (s, e) => {
                if (d == e.Device) {
                    d = null;
                }
            };
        }
        #region statics
        private static Gods.Steps.StepQueue queue = new Gods.Steps.StepQueue();

        [STAThread()]
        public static void Main() {
            var touched = new Dictionary<InputDevice, List<Point>>();
            var touching = 0;
            board.Loaded += delegate {
                queue.Add<Step1SetRadiusAndCount>();
                queue.Add<Step2AddPoints>();
                queue.Add<Step3Sort>();
                queue.Add<Step4PickKeys>();
                queue.Start();
            };
            board.KeyDown += (s, e) => {
                if (e.Key == Key.Escape) {
                    board.Close();
                }
            };
            board.TouchDown += (s, e) => {
                if (touched.Count > 1) {
                    touched.Clear();
                    touching = 0;
                    return;
                }
                touched[e.Device] = touched.Count == 0 ? null : new List<Point> { e.GetTouchPoint(board).Position };
                touching++;
            };
            board.TouchMove += (s, e) => {
                if (touched.ContainsKey(e.Device)) {
                    touched[e.Device]?.Add(e.GetTouchPoint(board).Position);
                }
            };
            board.TouchLeave += (s, e) => {
                touching--;
                if (touching < 1 && touched.Count == 2) {
                    var path = touched.Last().Value;
                    var vMax = path.Last() - path.First();
                    if (vMax.Length > board.Width / 2) {
                        queue.Move(vMax.X > 01);
                    }
                }
                if (touching < 1) {
                    touched.Clear();
                    touching = 0;
                }
            };
            new Application().Run(board);
        }
        #endregion
    }
}
