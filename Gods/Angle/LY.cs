using System.Collections.Generic;
using System.Text;

namespace Gods.Angle {
    public static class LY {

        public static string ToString<T>(this IEnumerable<T> source, object spliter) {
            var sb = new StringBuilder();
            foreach (var c in source) {
                sb.Append(c);
                sb.Append(spliter);
            }
            return sb.ToString();
        }

        public static string ToString(this byte[] source, bool with0X) {
            var sb = new StringBuilder(with0X ? "0x" : string.Empty);
            foreach (var b in source) {
                sb.Append(b.ToString("x"));
            }
            return sb.ToString();
        }
    }
}
