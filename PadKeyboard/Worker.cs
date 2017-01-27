using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

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
        private static finger[] fingers = new finger[10];
        public static Dictionary<string, Rect> rects = new Dictionary<string, Rect>();


        internal static int addFinger(Point p) {
            var name = rects.FirstOrDefault(r => r.Value.Contains(p)).Key;
            for (var i = 0; i < fingers.Length; i++) {
                if (fingers[i] == null) {
                    fingers[i] = new finger(i, name, p);
                    return i;
                }
            }
            throw new Exception();
        }

        internal static bool moveFinger(int id, Point p) {
            var name = rects.FirstOrDefault(r => r.Value.Contains(p)).Key;
            if (name == null) {
                removeFinger(id);
                return false;//TODO: send keys.
            }
            fingers.First(d => d?.id == id).add(name, p);
            return true;
        }

        internal static void removeAll() {
            for (var i = 0; i < fingers.Length; i++) {
                fingers[i] = null;
            }
        }
        internal static void removeFinger(int id) {
            for (var i = 0; i < fingers.Length; i++) {
                if (fingers[i]?.id == id) {
                    fingers[i] = null;
                    return;//TODO:send keys
                }
            }
        }

        private class finger {
            internal finger(int id, string n, Point p) {
                this.id = id;
                this.buttons.Add(new btn(n, p));
            }
            internal int id;
            internal List<btn> buttons = new List<btn>();
            internal void add(string n, Point p) {
                if (buttons[buttons.Count - 1].name == n) {
                    buttons[buttons.Count - 1].ps.Add(p);
                } else {
                    buttons.Add(new btn(n, p));
                }
            }

        }
        private class btn {
            internal string name;
            internal btn(string n, Point p) {
                name = n;
                ps.Add(p);
            }
            internal List<Point> ps = new List<Point>();
        }
    }
}