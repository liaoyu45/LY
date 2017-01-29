using Gods.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PadKeyboard.Steps {
    class Board : Window {

        public readonly static StepQueue<Board, Step> Queue = new StepQueue<Board, Step>();
        [STAThread()]
        public static void Main() {
            var ts = Assembly.GetExecutingAssembly().DefinedTypes;
            foreach (var item in ts) {
                if (item.IsSubclassOf(typeof(Step))) {
                    Queue.Steps.Add((Step)Activator.CreateInstance(item));
                }
            }
            new Application().Run(new Board());
        }

        internal readonly Grid content = new Grid { Name = "content_fuck" };

        public Board() {
            Content = content;
            content.Background = new SolidColorBrush { Color = new Color { A = 1 } };
            WindowStyle = WindowStyle.None;
            Background = new SolidColorBrush(new Color { });
            AllowsTransparency = true;
            Top = 0;
            Left = 0;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            ResizeMode = ResizeMode.NoResize;

            var ps = new Dictionary<int, List<Point>>();
            var fs = 0;
            TouchDown += (s, e) => {
                ps[ps.Count] = new List<Point> { e.GetTouchPoint(this).Position };
                fs++;
            };
            TouchMove += (s, e) => {
                ps[ps.Count].Add(e.GetTouchPoint(this).Position);
            };
            TouchLeave += (s, e) => {
                OnTouchMove(e);
                fs--;
                if (fs < 1 && ps.Count == 3) {
                    var ps2 = ps[2];
                    if (ps2.Count < 3 && (ps2[0] - ps2[ps2.Count - 1]).Length < Math.Max(Height, Width) / 2) {
                        return;
                    }
                    var r = new List<double>();
                    var v0 = ps2[1] - ps2[0];
                    var v1 = ps2[2] - ps2[1];
                    var max = v0.Y / v0.X;
                    var min = v1.Y / v1.X;
                    if (max < min) {
                        max = max + min;
                        min = max - min;
                        max = max - min;
                    }
                    for (var i = 2; i < ps2.Count - 1; i++) {
                        var v = (ps2[i + 1] - ps2[i]);
                        var k = v.Y / v.X;
                        if (k > min || k > max) {
                            return;
                        }
                        r.Add(k);
                    }
                    var avr = r.Average();
                    if (Math.Atan(Math.Abs(avr)) < Math.PI / 6 && v0.X * v1.X > 0) {
                        Queue.Move(v0.X > 0);
                    }
                    ThirdMove?.Invoke(avr);
                }
                if (fs < 1) {
                    ps.Clear();
                    fs = 0;
                }
            };
            Queue.Start(this);
        }

        public Action<double> ThirdMove;

        public Point GetPoint(TouchEventArgs e, double radius) {
            var p = e.GetTouchPoint(this).Position;
            p.Y = Math.Max(Math.Min(Height - radius, p.Y), radius);
            p.X = Math.Max(Math.Min(Width - radius, p.X), radius);
            return p;
        }
    }
}
