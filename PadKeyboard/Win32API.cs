using System;
using System.Runtime.InteropServices;

namespace PadKeyboard {
    class Win32API {
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        internal static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        internal static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        internal struct Point {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
