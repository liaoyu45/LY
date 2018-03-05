using System.Windows;

namespace PadKeyboard {
    class Trace : ITrace {
        public int Index { get; set; }
        public Vector Direction { get; set; }
        public bool Released { get; set; }
        public Point Center { get; set; }
        public double Radius { get; set; }

        public Trace() { }
        public Trace(ITrace t) {
            Index = t.Index;
            Released = t.Released;
            if (Released) {
                Direction = t.Direction; 
            }
            Radius = t.Radius;
        }
    }
}
