﻿using Gods.Steps;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    internal class Step2AddPoints : KeyStep {

        private Grid addPanel = Elements.BgA1Grid();
        private Grid effectPanel = new Grid();
        private double r => Beard.Radius;

        public Step2AddPoints() {
            Content.Children.Add(effectPanel);
            Content.Children.Add(addPanel);
            var ds = new Dictionary<InputDevice, Ellipse>();
            addPanel.TouchMove += (s, e) => {
                if (!ds.ContainsKey(e.Device)) {
                    return;
                }
                var ell = ds[e.Device];
                var p = Inside(e, ell);
                ell.Margin = new Thickness { Left = p.X, Top = p.Y };
            };
            addPanel.TouchLeave += (s, e) => {
                if (ds.ContainsKey(e.Device)) {
                    var ell = ds[e.Device];
                    var point = new Point(ell.Margin.Left + r, ell.Margin.Top + r);
                    foreach (var item in mapCenter()) {
                        if (item.Key == ell) {
                            continue;
                        }
                        if ((point - item.Value).Length < r * 2) {
                            effectPanel.Children.Remove(item.Key);
                        }
                    }
                    ds.Remove(e.Device);
                }
            };
            addPanel.TouchDown += (s, e) => {
                if (ds.ContainsKey(e.Device)) {//why
                    return;
                }
                var p = e.GetTouchPoint(addPanel).Position;
                foreach (var item in mapCenter()) {
                    if (Dicks.IsNear(item.Value, p, r)) {
                        if (!ds.ContainsValue(item.Key)) {
                            ds.Add(e.Device, item.Key);
                        }
                        return;
                    }
                }
                if (effectPanel.Children.Count >= Beard.KeysCount) {
                    effectPanel.Children.RemoveRange(Beard.KeysCount, 2198714);
                    return;
                }
                var ell = new Ellipse {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness { Left = p.X - r, Top = p.Y - r },
                    Width = r * 2,
                    Height = r * 2,
                    Fill = new SolidColorBrush(Colors.Black)
                };
                ds.Add(e.Device, ell);
                effectPanel.Children.Add(ell);
            };
        }

        protected override int Finish() {
            var ok = effectPanel.Children.Count == Beard.KeysCount;
            if (ok) {
                Beard.RawPoints = mapCenter().Values;
                return base.Finish();
            }
            return 0;
        }

        protected override int Cancel() {
            Beard.RawPoints = null;
            return base.Cancel();
        }

        public Dictionary<Ellipse, Point> mapCenter() {
            return effectPanel.Children.Cast<Ellipse>().ToDictionary(
                e => e,
                e => new Point(e.Margin.Left + r, e.Margin.Top + r));
        }
    }
}