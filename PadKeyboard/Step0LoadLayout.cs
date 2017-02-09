using Gods.Steps;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Step0LoadLayout : Step {
        private Ellipse r;

        protected override void Init(int offset) {
            Beard.Content = Elements.BgA1Grid();
            r = PieRadius.Create(222, 8, new Dictionary<double, Color> { { 0, Colors.Honeydew }, { 1, Colors.CadetBlue } });
            //r.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //r.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Beard.Content.Children.Add(r);
            Beard.Content.TouchMove += Content_TouchDown;
        }

        private void Content_TouchDown(object sender, System.Windows.Input.TouchEventArgs e) {
            var p = e.GetTouchPoint(Beard.Content).Position;
            r.Width = r.Height = (p - new System.Windows.Point(Beard.Width / 2, Beard.Height / 2)).Length * 2;
        }
    }
}
