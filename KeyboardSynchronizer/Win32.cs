using System;

namespace KeyboardSynchronizer {
    public static class Win32 {
        private static readonly IntPtr desktop = Win32ApiWrapper.GetDesktopWindow();

        public static void SwitchToThisWindow(this IntPtr self) {
            Win32ApiWrapper.SwitchToThisWindow(self.TillDesktop(), true);
        }

        public static IntPtr TillDesktop(this IntPtr self) {
            var p = Win32ApiWrapper.GetParent(self);
            if (p == IntPtr.Zero || p == desktop) {
                return self;
            }
            return TillDesktop(p);
        }

        public static bool SameOrBelongTo(this IntPtr self, IntPtr target) {
            if (self == target) {
                return true;
            }
            var p = Win32ApiWrapper.GetParent(self);
            if (p.ToInt32() == 0) {
                return false;
            }
            if (p == target) {
                return true;
            }
            return SameOrBelongTo(p, target);
        }
        //public static IEnumerable<IntPtr> EnumWindows() {
        //    var list = new List<IntPtr>();
        //    Win32ApiWrapper.EnumWindows((hWnd, lParam) => {
        //        list.Add(hWnd);
        //        return true;
        //    }, 0);
        //    return list;
        //}

        //public static Window GetWindowInfo(this IntPtr hWnd) {
        //    var w = new Window();
        //    var sb = new StringBuilder(256);
        //    w.IntPtr = hWnd;
        //    Win32ApiWrapper.GetWindowTextW(hWnd, sb, sb.Capacity);
        //    w.Text = sb.ToString();
        //    Win32ApiWrapper.GetClassNameW(hWnd, sb, sb.Capacity);
        //    w.Name = sb.ToString();
        //    int lpdwProcessId;
        //    Win32ApiWrapper.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
        //    if (lpdwProcessId == 0) {
        //        return w;
        //    }
        //    w.Process = Process.GetProcessById(lpdwProcessId);
        //    return w;
        //}

        //public static IEnumerable<IntPtr> EnumChildWindows(this IntPtr parentHwnd) {
        //    var list = new List<IntPtr>();
        //    Win32ApiWrapper.EnumChildWindows(parentHwnd, (hWnd, lParam) => {
        //        list.Add(hWnd);
        //        return true;
        //    }, 0);
        //    return list;
        //}

        //public static IEnumerable<IntPtr> EnumParentWindows(IntPtr hWnd) {
        //    var p = Win32ApiWrapper.GetParent(hWnd);
        //    if ((int)p > 0) {
        //        yield return p;
        //        foreach (var item in EnumParentWindows(p)) {
        //            yield return item;
        //        }
        //    }
        //}

        //public static IEnumerable<IntPtr> EnumParentWindowsUntil(IntPtr hWndChild, IntPtr hWndParent) {
        //    if (hWndChild == hWndParent) {
        //        goto empty;
        //    }
        //    var all = EnumParentWindows(hWndChild);
        //    if (all.Contains(hWndParent)) {
        //        return all;
        //    }
        //    empty:
        //    return Enumerable.Empty<IntPtr>();
        //}

    }
}
