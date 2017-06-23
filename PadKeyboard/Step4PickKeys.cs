using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PadKeyboard {
    class Step4PickKeys : KeyStep {
        const int cols = 20;
        const int rows = 11;


        private double w => SystemParameters.WorkArea.Width / cols;
        private double h => SystemParameters.WorkArea.Height / rows;

        public Step4PickKeys() {
            var pickedKeys = new WrapPanel();
            var previewer = new TextBlock {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = h / 3
            };
            Content.HorizontalAlignment = HorizontalAlignment.Left;
            Content.VerticalAlignment = VerticalAlignment.Top;
            Content.Background = new SolidColorBrush(Colors.White);
            for (var i = 0; i < cols; i++) {
                Content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (var i = 0; i < rows; i++) {
                Content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            Content.Children.Add(pickedKeys);
            Grid.SetColumn(pickedKeys, 7);
            Grid.SetRow(pickedKeys, 6);
            Grid.SetColumnSpan(pickedKeys, 6);
            Grid.SetRowSpan(pickedKeys, 5);
            var infoBox = new Grid { Background = new LinearGradientBrush(new GradientStopCollection(new[] { new GradientStop(), new GradientStop(Colors.Black, .5), new GradientStop { Offset = 1 } })) };
            Content.Children.Add(infoBox);
            Grid.SetColumn(infoBox, 7);
            Grid.SetRow(infoBox, 5);
            Grid.SetColumnSpan(infoBox, 6);
            infoBox.Children.Add(previewer);
            infoBox.TouchDown += (s, e) => {
                if (previewer.Tag != null || pickedKeys.Children.Count == 0) {
                    return;
                }
                Content.Children.Clear();
                var touchBoard = new TouchBoard { Background = new SolidColorBrush(new Color { A = 213 }) };
                previewer.Tag = touchBoard;
                touchBoard.Finished += (ss, ee) => {
                    touchBoard.Close();
                    previewer.Tag = null;
                };
                touchBoard.ShowDialog();
                touchBoard.WithEffect = true;
                touchBoard.Layout(Beard.Radius, Beard.OrderedPoints);
            };
            Action<Grid> preview = k => {
                k.TouchEnter += (s, e) => { previewer.Text = ((s as Grid).Children[0] as TextBlock).Text; };
                k.TouchLeave += (s, e) => { previewer.Text = string.Empty; };
            };
            Action<object> addKey = tag => {
                if (pickedKeys.Children.Count > 6 * 4 - 1) {
                    return;
                }
                var k = ShadowBox.Create(new GradientStop(Colors.White, 0), new GradientStop(Colors.Black, 1));
                preview(k);
                k.Tag = tag;
                k.Width = w;
                k.Height = h;
                k.Children.Add(new TextBlock {
                    Text = tag.ToString(),
                    FontSize = h / 3,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                pickedKeys.Children.Add(k);
                Action<bool> changeArrow = d => {
                    ShadowBox.ShowCorner(k, d ? 3 : 1, b =>
                        b.Viewbox = new Rect {
                            Y = d ? .25 : -.25, Width = 1, Height = 1
                        });
                };
                changeArrow(false);
                InputDevice temp = null;
                k.TouchDown += (s, e) => {
                    if (temp == null) {
                        temp = e.Device;
                    }
                };
                Content.TouchLeave += (s, e) => {
                    if (temp != e.Device) {
                        return;
                    }
                    temp = null;
                    var y = e.GetTouchPoint(k).Position.Y;
                    if (Math.Abs(y - h / 2) < h / 6) {
                        pickedKeys.Children.Remove(k);
                    } else {
                        changeArrow(y > h / 2);
                    }
                };
            };
            var j = 1;
            for (var i = 0; i < rows * cols; i++) {
                var ci = i % cols;
                var ri = i / cols;
                if (ri > 4 && ci > 5 && ci < 14) {
                    continue;
                }
                var k = ShadowBox.Create(new GradientStop { Color = Colors.Black }, new GradientStop { Offset = 1 });
                k.Tag = (Key)j;
                k.Height = h;
                k.Width = w;
                k.Children.Add(new TextBlock {
                    Text = k.Tag.ToString(),
                    FontSize = h / 3,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                Elements.AddClick(k, addKey, k.Tag);
                preview(k);
                Content.Children.Add(k);
                Grid.SetColumn(k, ci);
                Grid.SetRow(k, ri);
                j++;
            }
        }
    }
}
