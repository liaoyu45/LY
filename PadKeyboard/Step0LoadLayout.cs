using Gods.Steps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PadKeyboard {
    class Step0LoadLayout : Step {
        protected override void Init(int offset) {
            var layouts = Directory.EnumerateFiles("layouts").Select(load).Where(l => l != null);
        }

        private object load(string path) {
            var f = File.OpenText(path);
            var line = f.ReadLine();
            return 1;
        }
    }
}
