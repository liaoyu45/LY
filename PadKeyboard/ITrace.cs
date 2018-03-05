using System.Windows;

namespace PadKeyboard {
    internal interface ITrace {
        int Index { get; }
        Vector Direction { get; }
        double Radius { get; }
        bool Released { get; }
    }
}
