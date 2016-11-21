using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Gods.System {
    public static class Him {
        public static IPAddress GetLocalIP() {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
                if (ip.AddressFamily.ToString() == "InterNetwork") {
                    return ip;
                }
            }
            return null;
        }

        /// <summary>
        /// 扩展 DirectoryInfo.EnumerateFiles 的 searchPattern。
        /// </summary>
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo source, params string[] searchPatterns) {
            return searchPatterns.SelectMany(p => source.EnumerateFiles("*." + p));
        }

        /// <summary>
        /// 扩展 Directory.EnumerateFiles 的 searchPattern。
        /// </summary>
        public static IEnumerable<string> EnumerateFiles(string folder, params string[] searchPatterns) =>
            searchPatterns.SelectMany(p => Directory.EnumerateFiles(folder, "*." + p));
    }
}
