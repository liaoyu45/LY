using System.Windows.Media;

namespace PadKeyboard {
    class Brushes {

        public static readonly Brush picked = new RadialGradientBrush(Colors.White, Colors.Black);
        public static readonly Brush pending = new RadialGradientBrush(Colors.Black, Colors.White);
    }
}
