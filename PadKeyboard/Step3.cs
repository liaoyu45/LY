using Gods.Steps;
using System.Windows.Controls;

namespace PadKeyboard {
    class Step3 : Step {

        private Grid content = new Grid();

        protected override void Init(int offset) {
            Beard.Board.Content = content;

        }
    }
}
