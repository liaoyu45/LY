using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PadKeyboard {
    class Step4Control : Grid {
        public Step4Control() {
            Background = new SolidColorBrush(new Color { A = 1 });
            var traces = new List<Trace>();
            var reseters = new List<Action>();
            Action<Trace> add = trace => {
                traces.Add(trace);
                return;
                if (traces.GroupBy(t => t.Center).All(t => t.Last().Released)) {
                    MessageBox.Show("");
                    reseters.ForEach(r => r());
                }
            };
            var minLength = Beard.Radius / 3 * 2;
            for (var i = 0; i < Beard.RawPoints.Count; i++) {
                reseters.Add(this.MoveEffect(new Trace {
                    Radius = Beard.Radius,
                    Center = Beard.RawPoints[i]
                }, minLength, add, null, add));
            }
        }
    }
}
