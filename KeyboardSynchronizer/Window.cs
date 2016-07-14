using System;
using System.Diagnostics;

namespace KeyboardSynchronizer {
    public class Window {
        public IntPtr IntPtr { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public Process Process { get; set; }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            return IntPtr == ((Window)obj).IntPtr;
        }

        public override int GetHashCode() {
            return IntPtr.ToInt32();
        }
    }
}
