using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Linq {
    public static class Sort {
        /// <summary>
        /// Map orignal indexes to sorted indexes, eg: SI2I([2,5,9,4,6,7])=[0(0,2),3(1,4),1(2,5),4(3,6),5(4,7),2(5,9)].
        /// </summary>
        /// <returns></returns>
        public static int[] SI2I<T>(this List<T> s, Func<T, int> by) {
            var r = new List<int>();
            var t = new List<T>(s);
            while (r.Count < s.Count) {
                var f = 0;
                var v = by(t[0]);
                for (var i = 1; i < t.Count; i++) {
                    var n = by(t[i]);
                    if (n < v) {
                        f = i;
                        v = n;
                    }
                }
                var ts = t[f];
                var ii = s.IndexOf(ts);
                while (r.Contains(ii)) {
                    ii = s.IndexOf(ts, ii + 1);
                }
                r.Add(ii);
                t.RemoveAt(f);
            }
            return r.ToArray();
        }
        /// <summary>
        /// Map sorted indexes to orignal indexes, see <see cref="SI2I{T}(List{T}, Func{T, int})"/>.
        /// </summary>
        public static int[] I2SI<T>(this List<T> s, Func<T, int> by) {
            return SI2I(SI2I(s, by).ToList(), i => i);
        }
    }
}
