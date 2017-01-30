using Gods.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    internal class Step1SetRadiusAndCount : Step {

        private Grid content = new Grid();
        private Grid addPanel = new Grid { Background = new SolidColorBrush { Color = new Color { A = 1 } } };
        private Grid effectPanel = new Grid();
        private Grid countPanel = new Grid {
            Background = new SolidColorBrush { Color = new Color { A = 11 } },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Height = Beard.Height / 3
        };
        private Grid howtoSay = new Grid {
            Background = new SolidColorBrush { Color = new Color { A = 255, G = 255 } },
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Left,
            Height = Beard.Height / 3,
            Width = Beard.Height / 3
        };
        private TextBlock countText = new TextBlock {
            Text = Beard.KeysCount.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 44,
            Foreground = new SolidColorBrush { Color = Color.FromScRgb(1, 1, 1, 1) }
        };

        private double MaxLeft {
            get { return Beard.Width - howtoSay.Width; }
        }
        private Color shownColor = new Color { A = 255, G = 255 };

        public Step1SetRadiusAndCount() {
            content.Children.Add(effectPanel);
            content.Children.Add(addPanel);
            content.Children.Add(countPanel);
            countPanel.Children.Add(howtoSay);
            howtoSay.Children.Add(countText);

            howtoSay.Margin = new Thickness { Left = MaxLeft * Beard.KeysCount / Beard.KeysCountMax };
            setColor();

            var fs = new List<f>();
            countPanel.TouchMove += (s, e) => {
                e.Handled = true;
                var p = e.GetTouchPoint(countPanel).Position;
                var left = Math.Max(0, Math.Min(MaxLeft, p.X - howtoSay.Width / 2));
                howtoSay.Margin = new Thickness { Left = left };
                Beard.KeysCount = (int)(left / MaxLeft * Beard.KeysCountMax);
                setColor();
            };
            addPanel.TouchMove += (s, e) => {
                var f = fs.FirstOrDefault(p => p.d == e.Device);
                if (f == null) {
                    return;
                }
                f.p = e.GetPoint();
                if (fs.Count == 2) {
                    Beard.Radius = Math.Min(Beard.MaxRadius, (fs[0].p - fs[1].p).Length / 2);
                    setColor();
                    fs[0].e.Width = fs[1].e.Width = fs[0].e.Height = fs[1].e.Height = Beard.Radius * 2;
                    (f.e.Fill as SolidColorBrush).Color = shownColor;
                }
                f.e.Margin = new Thickness { Left = f.p.X - Beard.Radius, Top = f.p.Y - Beard.Radius };
            };
            addPanel.TouchLeave += (s, e) => {
                var f = fs.FirstOrDefault(p => p.d == e.Device);
                if (f == null) {
                    return;
                }
                fs.Remove(f);
                effectPanel.Children.Remove(f.e);
            };
            addPanel.TouchDown += (s, e) => {
                if (fs.Count == 2) {
                    return;
                }
                var p = e.GetPoint();
                var ell = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - Beard.Radius, Top = p.Y - Beard.Radius },
                    Width = Beard.Radius * 2,
                    Height = Beard.Radius * 2,
                    Fill = new SolidColorBrush(new Color { A = 111 })
                };
                fs.Add(new f { d = e.Device, e = ell, p = p });
                effectPanel.Children.Add(ell);
            };
        }

        protected override bool Finish() {
            effectPanel.Children.Clear();
            addPanel.Children.Clear();
            return true;
        }

        protected override bool Cancel() {
            Beard.Board.Close();
            return true;
        }

        protected override void Init(int offset) {
            effectPanel.Children.Clear();
            addPanel.Children.Clear();
            Beard.Board.Content = content;

        }

        private void setColor() {
            countText.Text = Beard.KeysCount + string.Empty;
            shownColor.G = (byte)Math.Min(255, 255.0 * Beard.KeysCount / Beard.KeysCountDefault);
            shownColor.B = (byte)(Math.Max(0, (Beard.KeysCount - Beard.KeysCountDefault) / (double)(Beard.KeysCountMax - Beard.KeysCountDefault) * 255));
            shownColor.R = (byte)(255 - (byte)Math.Max(0, (255 * Beard.Radius / Beard.MaxRadius)));
            (howtoSay.Background as SolidColorBrush).Color = shownColor;
        }

        class f {
            public InputDevice d;
            public Ellipse e;
            public Point p;
        }
    }
}