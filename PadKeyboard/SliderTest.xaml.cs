using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PadKeyboard {
    /// <summary>
    /// SliderTest.xaml 的交互逻辑
    /// </summary>
    public partial class SliderTest : Window {

        public SliderTest() {
            InitializeComponent();
            Loaded += SliderTest_Loaded;
        }

        private void SliderTest_Loaded(object sender, RoutedEventArgs e) {
            var t = (qq.Template.FindName("PART_Track", qq)) as Track;
            var r = t.Thumb.Template.FindName("e", t.Thumb) as Rectangle;
            r.Width = 444;
            //r.TouchDown += delegate { MessageBox.Show("lkewjrkewljr"); };
            Console.WriteLine(t);
        }

        private void Qq_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Console.WriteLine(qq.Template);
        }
    }
}
