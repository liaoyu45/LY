using System.Windows.Media;

namespace PadKeyboard {
    class Brushes {

        public static readonly Brush picked = new RadialGradientBrush(Colors.White, Colors.Black);
        public static readonly Brush pending = new RadialGradientBrush(Colors.Black, Colors.White);
        public static readonly Brush R2L = new LinearGradientBrush(Colors.White, Colors.Black, 0);
        public static readonly Brush L2R = new LinearGradientBrush(Colors.White, Colors.Black, 0);
    }
}
