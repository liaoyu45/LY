using System.Windows;
using System.Windows.Input;

namespace PadKeyboard {
    /// <summary>
    /// KeysSelector.xaml 的交互逻辑
    /// </summary>
    public partial class KeysSelector : Window {
        public KeysSelector() {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
            Closing += (s, e) => e.Cancel = true;
            Key a;
        }
    }
}
