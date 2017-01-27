using System.Windows;
using System.Windows.Input;

namespace PadKeyboard {
    /// <summary>
    /// TesT.xaml 的交互逻辑
    /// </summary>
    public partial class TesT : Window {
        public TesT() {
            InitializeComponent();
        }

        private void Button_TouchDown(object sender, TouchEventArgs e) {
            tt.Text += "--b";
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e) {
            tt.Text += "--g";
        }
    }
}
