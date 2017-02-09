using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections;

namespace PadKeyboard {
    class Dicks : IEnumerable<Dick> {
        private List<Dick> ds = new List<Dick>();
        private readonly double radius;

        public Dicks(double r) {
            radius = r;
        }
        public Dick this[Point p] {
            get {
                return ds.Find(d => IsNear(d.point, p, radius));
            }
        }
        public bool add(Point p, Gests g, Key k) {
            var dk = ds.Find(d => d.point == p);
            if (dk == null) {
                ds.Add(new Dick {
                    point = p,
                    gests = new Dictionary<Gests, List<Key>> { { g, new List<Key> { k } } }
                });
            } else if (dk.gests[g].Count(kk => kk == k) < 2) {
                dk.gests[g].Add(k);
            } else {
                return false;
            }
            return true;
        }

        public static bool IsNear(Point p0, Point p1, double length) {
            return (p0 - p1).Length < length;
        }

        public IEnumerator<Dick> GetEnumerator() {
            return ds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
    class Dick {
        public Point point;
        public Dictionary<Gests, List<Key>> gests;
    }
    enum Gests {
        Click = -1,
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3,
    }
}
