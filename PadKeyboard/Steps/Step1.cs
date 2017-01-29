using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard.Steps {
    internal class Step1 : Step {

        protected override int Index { get; set; } = 1;

        private readonly double radius = SystemParameters.WorkArea.Height / 12;
        private readonly Grid addPanel = new Grid { Background = new SolidColorBrush { Color = new Color { A = 1 } } };
        private readonly Grid effectPanel = new Grid();

        protected override void Finish() {
            Target.content.Children.Clear();
            effectPanel.Children.Clear();
            addPanel.Children.Clear();
        }

        protected override void Cancel() {
            Finish();
        }

        protected override void Initiate(object result, bool direction) {
            var radius = this.radius;
            Target.content.Children.Add(effectPanel);
            Target.content.Children.Add(addPanel);
            var fs = new List<f>();
            addPanel.TouchMove += (s, e) => {
                var f = fs.FirstOrDefault(p => p.d == e.Device);
                if (f == null) {
                    return;
                }
                f.p = Target.GetPoint(e, radius);
                if (fs.Count == 2) {
                    radius = (fs[0].p - fs[1].p).Length / 2;
                    Result = radius;
                    fs[0].e.Width = fs[1].e.Width = fs[0].e.Height = fs[1].e.Height = radius * 2;
                }
                f.e.Margin = new Thickness { Left = f.p.X - radius, Top = f.p.Y - radius };
            };
            addPanel.TouchLeave += (s, e) => {
                fs.Clear();
            };
            addPanel.TouchDown += (s, e) => {
                if (fs.Count == 2) {
                    return;
                }
                var p = Target.GetPoint(e, radius);
                var ell = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - radius, Top = p.Y - radius },
                    Width = radius * 2,
                    Height = radius * 2,
                    Fill = new SolidColorBrush(new Color { A = 111 })
                };
                fs.Add(new f { d = e.Device, e = ell, p = p });
                effectPanel.Children.Add(ell);
            };
        }

        class f {
            public InputDevice d;
            public Ellipse e;
            public Point p;
        }
    }
}