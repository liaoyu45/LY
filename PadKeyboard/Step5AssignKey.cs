using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PadKeyboard {
    class Step5AssignKey : Gods.Steps.Step {
        private readonly Grid content = new Grid { Background = new SolidColorBrush(Colors.White) };
        private readonly WrapPanel keys = new WrapPanel();

        public Step5AssignKey() {
            var gridLength = new GridLength(1, GridUnitType.Star);
            const int cols = 20, rows = 11;
            var w = Beard.Width / cols;
            var h = Beard.Height / rows;
            /*20*5+6*6=172
            11111111111111111111
            11111111111111111111
            11111111111111111111
            11111111111111111111
            11111111111111111111
            111111--------111111
            111111========111111
            111111========111111
            111111========111111
            111111========111111
            111111========111111
            */
            for (var i = 0; i < cols; i++) {
                content.ColumnDefinitions.Add(new ColumnDefinition { Width = gridLength });
            }
            for (var i = 0; i < rows; i++) {
                content.RowDefinitions.Add(new RowDefinition { Height = gridLength });
            }
            content.Children.Add(keys);
            Grid.SetColumn(keys, 6);
            Grid.SetRow(keys, 6);
            Grid.SetColumnSpan(keys, 8);
            Grid.SetRowSpan(keys, 5);
            var preview = new Func<TextBlock>(delegate {
                var b = new Grid();
                content.Children.Add(b);
                Grid.SetColumn(b, 6);
                Grid.SetRow(b, 5);
                Grid.SetColumnSpan(b, 8);
                b.Children.Add(new TextBlock {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                return b.Children[0] as TextBlock;
            })();
            Action<Grid> change = k => {
                var down = (bool)k.Tag;
                k.Tag = !down;
                BoxShadow.ShowCorner(k, down ? 1 : 3, b => {
                    var rect = new Rect { Y = !down ? .25 : -.25, Width = 1, Height = 1 };
                    b.Viewbox = rect;
                });
            };
            EventHandler<TouchEventArgs> toggleState = (s, e) => change(s as Grid);
            EventHandler<TouchEventArgs> showFull = (s, e) => preview.Text = ((s as Grid).Children[0] as TextBlock).Tag + string.Empty;
            EventHandler<TouchEventArgs> hideFull = (s, e) => preview.Text = string.Empty;
            Action<Grid> seeFull = g => {
                g.TouchEnter += showFull;
                g.TouchLeave += hideFull;
            };
            var gs = new[] { new GradientStop(), new GradientStop { Color = Colors.Black, Offset = 1 } };
            EventHandler<TouchEventArgs> addKey = (s, e) => {
                if (keys.Children.Count > 8 * 5 - 1) {
                    return;
                }
                var k = BoxShadow.Create(gs);
                k.Width = w;
                k.Height = h;
                var txt = (s as Grid).Children[0] as TextBlock;
                k.Children.Add(new TextBlock {
                    Text = txt.Text,
                    FontSize = h / 2,
                    Tag = txt.Tag,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                k.Tag = false;
                change(k);
                keys.Children.Add(k);
                seeFull(k);
                k.TouchUp += toggleState;
            };
            var v = (int[])Enum.GetValues(typeof(Key));
            var n = Enum.GetNames(typeof(Key));
            var j = 1;
            for (var i = 0; i < rows * cols; i++) {
                var c = i % cols;
                var r = i / cols;
                if (r > 4 && c > 5 && c < 14) {
                    continue;
                }
                var nj = n[j];
                var k = Elements.ShadowButton(t => {
                    t.HorizontalAlignment = HorizontalAlignment.Center;
                    t.FontSize = h / 2;
                    t.Text = nj[0].ToString();
                    if (nj.Length > 1) {
                        t.Text += nj[1];
                    }
                    t.Tag = (Key)v[j];
                });
                k.TouchUp += addKey;
                seeFull(k);
                j++;
                content.Children.Add(k);
                Grid.SetColumn(k, c);
                Grid.SetRow(k, r);
            }
        }


        protected List<Key> Keys { get; } = new List<Key>();

        protected List<KeyValuePair<Key, bool>> Filter() {
            var r = new List<KeyValuePair<Key, bool>>();
            foreach (var item in Keys) {
                var c = r.Count(i => i.Key == item);
                if (c > 2) {
                    continue;
                }
                r.Add(new KeyValuePair<Key, bool>(item, c == 0));
            }
            r.AddRange(Keys.GroupBy(i => i).Where(g => g.Count() == 1).ToDictionary(g => g.Key, g => false));
            return r;
        }

        protected override void Init(int offset) {
            Beard.Content = content;
        }

        public delegate void KeyQueuesEventHandler(object sender, KeyQueuesEventArgs e);
        public class KeyQueuesEventArgs : EventArgs {
            public List<KeyValuePair<Key, bool>> Keys { get; set; }
        }
    }
}
