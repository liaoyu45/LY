using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardSynchronizer {
    public static class Worker {
        #region fields
        static int kbhHook;
        static Yeller specifier;
        static string moduleName;
        static List<KeyValuePair<IntPtr, bool>> AllWindows { get; } = new List<KeyValuePair<IntPtr, bool>>();
        public static Func<IEnumerable<KeyValuePair<IntPtr, bool>>> Collector;
        public static Action<KeysQueue> SpecifyFunction = new Action<KeysQueue>(executeKeysQueue);
        public static Func<bool> Preparing;
        public static event Action<bool> Prepared;
        public static event Action Started;
        public static event Action FailToStart;
        public static event Action<IntPtr> WorkingNotice;
        public static event Action<IntPtr> WindowLost;
        public static event Action Stopped;
        public static event Action Terminaled;
        public static event Action SendKeyUpChanged;

        /// <summary>
        /// Only when one of these windows is forground, keyboard sychronize events can be sent.
        /// </summary>
        public static IEnumerable<IntPtr> MainWindows { get; private set; }
        public static bool SendKeyUp { get; set; } = true;
        public static bool Working { get; private set; }
        public static List<Yeller> Yellers { get; } = new List<Yeller>();
        public static List<Keys> ExcludedKeys { get; } = new List<Keys>();
        #endregion

        public static void Prepare() {
            if (Preparing?.Invoke() == false) {
                return;
            }
            moduleName = moduleName ?? Process.GetCurrentProcess().MainModule.ModuleName;
            var intPtr = Win32ApiWrapper.GetModuleHandle(moduleName);
            kbhHook = Win32ApiWrapper.SetWindowsHookEx(WinApiConsts.HookType.WH_KEYBOARD_LL, hook, intPtr, 0);
            Prepared?.BeginInvoke(kbhHook != 0, null, null);
        }

        static SetWindowsHookEx hook = (nCode, wParam, lParam) => {
            var kbhs = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            var key = kbhs.vkCode;
            switch (wParam) {
                case WinApiConsts.Msg.WM_KEYDOWN:
                case WinApiConsts.Msg.WM_SYSKEYDOWN:
                    if (specifier != null) {
                        if (key == specifier.ShutUpKey) {
                            if (specifier.IsYelling) {
                                if (isALT(key)) {
                                    specifier.Yell(specifier.ShutUpKey);
                                }
                            } else {
                                specifier.StartToYell();
                            }
                        }
                        if (specifier.IsYelling) {
                            return 1;
                        }
                    }
                    keyDown(key);
                    break;
                case WinApiConsts.Msg.WM_KEYUP:
                case WinApiConsts.Msg.WM_SYSKEYUP:
                    if (specifier != null) {
                        specifier.Yell(key);
                        if (specifier.IsYelling || specifier.ShutUpKey == key) {
                            return 1;
                        }
                    }
                    keyUp(key);
                    break;
                default:
                    break;
            }
            return Win32ApiWrapper.CallNextHookEx(kbhHook, nCode, wParam, lParam);
        };

        static void keyDown(Keys key) {
            perform(WinApiConsts.Msg.WM_KEYDOWN, key);
            Yellers.ForEach(y => {
                if (key == y.ShutUpKey) {
                    y.StartToYell();
                } else {
                    if (isALT(key)) {
                        y.Yell(y.ShutUpKey);
                    }
                }
            });
        }

        static void keyUp(Keys key) {
            Yellers.ForEach(y => {
                if (isALT(key)) {
                    y.Yell(y.ShutUpKey);
                } else {
                    y.Yell(key);
                }
            });
            if (!SendKeyUp) {
                return;
            }
            perform(WinApiConsts.Msg.WM_KEYUP, key);
        }

        static bool isALT(Keys k) {
            return k == Keys.LMenu || k == Keys.RMenu;
        }

        static void perform(int action, Keys key) {
            if (!Working) {
                return;
            }
            if (ExcludedKeys.Contains(key)) {
                return;
            }
            var fore = Win32ApiWrapper.GetForegroundWindow();
            if (!MainWindows.Any(w => w.SameOrBelongTo(fore))) {
                return;
            }
            WorkingNotice?.BeginInvoke(fore, null, null);
            for (int i = AllWindows.Count - 1; i >= 0; i--) {
                var item = AllWindows[i];
                var w = item.Key;
                if (w.SameOrBelongTo(fore)) {
                    continue;
                }
                var r = Win32ApiWrapper.PostMessage(w, action, (int)key, 0);
                if (r == 0) {
                    AllWindows.Remove(item);
                    WindowLost?.BeginInvoke(w, null, null);
                }
            }
        }

        public static void AddSpecifyKey(Keys startKey) {
            if (startKey == Keys.None) {
                specifier = null;
                return;
            }
            var s = Yellers.FirstOrDefault(y => y.Name == nameof(startKey));
            if (s != null) {
                if (s.IsYelling) {
                    return;
                }
                s.ShutUpKey = startKey;
                return;
            }
            specifier = new Yeller(startKey);
            specifier.Name = nameof(startKey);
            specifier.Yelling += (y, e) => {
                switch (e.Index) {
                    case Yeller.STARTED:
                        y.Tag = new KeysQueue();
                        break;
                    case Yeller.TERMINALED:
                        SpecifyFunction((KeysQueue)y.Tag);
                        y.Tag = null;
                        break;
                    default:
                        var tag = (KeysQueue)y.Tag;
                        tag.Insert(e.Key);
                        y.Tag = tag;
                        return;
                }
            };
        }

        static void executeKeysQueue(KeysQueue queue) {
            Collect();
            switch (queue.Id) {
                case -1:
                    if (queue.Index > -1) {
                        if (AllWindows.Count > queue.Index) {
                            var w = AllWindows[queue.Index].Key;
                            if (!w.SameOrBelongTo(Win32ApiWrapper.GetForegroundWindow())) {
                                w.SwitchToThisWindow();
                                WorkingNotice?.BeginInvoke(w, null, null);
                            }
                        }
                    }
                    break;
                case 0:
                    if (queue.Index == -1) {//Toggle: NaN key
                        if (Working) {
                            Stop();
                        } else {
                            Start();
                        }
                    } else {//PostMessage: NaN key + N keys
                        if (AllWindows.Count > queue.Index) {
                            var w = AllWindows[queue.Index].Key;
                            if (queue.Starter == queue.Empty) {//for accuration.
                                var k = (int)queue.IdToggler;
                                Win32ApiWrapper.PostMessage(w, WinApiConsts.Msg.WM_KEYDOWN, k, 0);
                                if (SendKeyUp) {
                                    Win32ApiWrapper.PostMessage(w, WinApiConsts.Msg.WM_KEYUP, k, 0);
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    SendKeyUp = !SendKeyUp;
                    try {
                        SendKeyUpChanged?.BeginInvoke(null, null);
                    } catch {
                    }
                    break;
                default://ShowWindow: SW_MINIMIZE/SW_NORMAL
                    if (queue.Starter == queue.Empty) {//for accuration
                        if (queue.Index == -1) {//for accuration
                            var c = Win32ApiWrapper.GetForegroundWindow();
                            if (AllWindows.Any(w => w.Key.SameOrBelongTo(c))) {//find current
                                var iconic = AllWindows.Any(w => Win32ApiWrapper.IsIconic(w.Key.TillDesktop()));
                                var nCmdShow = iconic ? WinApiConsts.nCmdShow.SW_NORMAL : WinApiConsts.nCmdShow.SW_MINIMIZE;
                                foreach (var item in AllWindows.Select(w => w.Key)) {
                                    if (item.SameOrBelongTo(c)) {//keep current
                                        continue;
                                    }
                                    Win32ApiWrapper.ShowWindow(item.TillDesktop(), nCmdShow);
                                }
                                c.SwitchToThisWindow();
                            }
                        }
                    }
                    break;
            }
        }

        public static void Terminal() {
            if (kbhHook != 0) {
                Win32ApiWrapper.UnhookWindowsHookEx(kbhHook);
                kbhHook = 0;
            }
            Terminaled?.BeginInvoke(null, null);
        }

        /// <summary>
        /// Starts to sychronize keyboard events, will be failed if <see cref="MainWindows"/> is null or empty.
        /// </summary>
        public static void Start() {
            if (Working) {
                return;
            }
            Collect();
            MainWindows = AllWindows.Where(w => w.Value).Select(w => w.Key);
            Working = MainWindows.Any();
            if (!Working) {
                FailToStart?.BeginInvoke(null, null);
                return;
            }
            Started?.BeginInvoke(null, null);
        }

        public static void Collect() {
            if (Collector == null) {
                return;
            }
            var windows = Collector();
            if (windows.Any()) {
                AllWindows.Clear();
                AllWindows.AddRange(windows);
            }
        }

        public static void Stop() {
            if (!Working) {
                return;
            }
            Working = false;
            Stopped?.BeginInvoke(null, null);
        }
    }
}