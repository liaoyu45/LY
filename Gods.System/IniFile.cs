using System.Runtime.InteropServices;
using System.Text;

namespace Gods.System {
    public class IniFile {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string path);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string path);

        public string path;
        public IniFile(string path) {
            this.path = path;
        }
        public int MaxLength { get; set; } = 500;

        public string this[string section, string key] {
            set {
                WritePrivateProfileString(section, key, value, path);
            }
            get {
                var temp = new StringBuilder(MaxLength);
                GetPrivateProfileString(section, key, string.Empty, temp, MaxLength, path);
                return temp.ToString();
            }
        }
    }
}
