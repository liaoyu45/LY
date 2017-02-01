using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PadKeyboard {
    class Step4AssignKeys : Gods.Steps.Step {
        private IEnumerable<Ellipse> points;
        private Grid content;

        private readonly Brush picked = new RadialGradientBrush(Colors.White, Colors.Black);

        public Step4AssignKeys() {
            content = Beard.BgA1Grid();
        }

        protected override void Init(int offset) {
            Beard.Content = content;
            points = Beard.Visual<Ellipse>(Beard.OrderedPoints);
            foreach (var item in points) {
                item.Fill = picked;
                content.Children.Add(item);
            }
        }
    }
}
