using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
namespace KeyboardSynchronizer {
    public class Win32ApiWrapper {

        public delegate bool EnumFunc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardLayout(int idThread);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("Gdi32.dll")]
        public extern static int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public extern static IntPtr GetWindow(IntPtr hWnd, int wCmd);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SwitchToThisWindow(IntPtr hWnd, bool altTab);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumFunc lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumFunc lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        static public extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hwndParent, int hwndChildAfter, IntPtr wParam, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern int GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ScreenToClient(IntPtr hWnd, ref Rectangle rect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ClientToScreen(IntPtr hWnd, ref Rectangle rect);

        [DllImport("user32.dll")]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int threadId, EnumFunc pfnEnum, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("Kernel32.dll")]
        public extern static IntPtr OpenProcess(int fdwAccess, int fInherit, uint IDProcess);

        [DllImport("kernel32.dll")]
        public static extern uint GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpszFileName, int nSize);

        [DllImport("user32.dll")]
        public extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        [DllImport("user32.dll")]
        public extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, SetWindowsHookEx lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);
    }

    public delegate int SetWindowsHookEx(int a, int b, IntPtr c);
}