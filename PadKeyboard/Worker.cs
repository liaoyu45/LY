using System;

namespace PadKeyboard {
    class Worker {
        const int WS_EX_TRANSPARENT = 0x20;
        const int GWL_EXSTYLE = (-20);
        internal static void MakeTransparent(IntPtr target, bool state) {
            if (state) {
                if (oldOne == 0) {
                    oldOne = Win32API.GetWindowLong(target, GWL_EXSTYLE);
                }
                Win32API.SetWindowLong(target, GWL_EXSTYLE, oldOne | WS_EX_TRANSPARENT);
            } else {
                if (oldOne == 0) {
                    return;
                }
                Win32API.SetWindowLong(target, GWL_EXSTYLE, oldOne);
            }
        }
        private static uint oldOne;
    }
}