using System.Linq;
using System.Windows.Forms;

namespace KeyboardSynchronizer {
    public class KeysQueue : FuncQueue<Keys> {
        public KeysQueue() {
            this.Restarter = new[] { Keys.S, Keys.T, Keys.O, Keys.P };
        }
        static Keys[] numbers = { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9 };

        public override Keys Empty {
            get { return Keys.None; }
        }

        protected override bool Equals(Keys a, Keys b) {
            return a == b;
        }

        protected override int CreateNewIndex(Keys t) {
            var v = (Index > -1 ? Index.ToString() : string.Empty) + t.ToString().Last();
            try {
                return int.Parse(v);
            } catch {
                return Index;
            }
        }

        protected override bool IsIndexElement(Keys t) {
            return numbers.Contains(t);
        }
    }
}
