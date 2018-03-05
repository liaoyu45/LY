using System.Collections.Generic;
using System.Windows;

namespace PadKeyboard {
    static class Beard {

        public const int KeysMin = 16;
        public const int KeysMax = 64;
        public const int KeysRange = 48;

        public static int KeysCount = KeysMin;
        public static double Radius;

        public static IEnumerable<Point> RawPoints;
        public static List<Point> OrderedPoints;

    }
}
