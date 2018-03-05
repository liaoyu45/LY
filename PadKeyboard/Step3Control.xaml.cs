using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PadKeyboard {
    public partial class Step3Control : Grid {
        public Step3Control() {
            InitializeComponent();
            var width = 0d;
            Loaded += (s, e) => width = pickedKeys.ActualHeight;
            allKeysBoard.Children.OfType<StackPanel>().SelectMany(s => s.Children.OfType<SquareButton>()).ToList().ForEach(item => {
                item.Tag = 1;//TODO:set keycode
                var p = new Point();//position
                item.Loaded += (s, e) => p = item.TranslatePoint(p, this);
                item.TouchDown += (ds, de) => {
                    item.IsHitTestVisible = false;
                    var ing = false;
                    var i = -1;//insert
                    var dp = de.GetTouchPoint(this).Position;//down position
                    var m = new SquareButton {//moving button
                        Content = item.Content,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    EventHandler<TouchEventArgs> move = null, leave = null;
                    TouchMove += move = (s, e) => {
                        if (de.Device != e.Device) {
                            return;
                        }
                        var v = e.GetTouchPoint(this).Position - dp;
                        if (ing) {
                            var n = p + v;//new position
                            m.Margin = new Thickness {
                                Left = n.X,
                                Top = n.Y
                            };
                            var tp = e.GetTouchPoint(this).Position;
                            if (tp.Y > pickedKeys.TranslatePoint(new Point(), this).Y) {//TODO:add new effect should.
                                if (pickedKeys.Children.Count == 0) {
                                    i = 0;
                                } else {
                                    var t = pickedKeys.Children.OfType<SquareButton>().Select(b => new {
                                        b,
                                        b.TranslatePoint(new Point(), this).X
                                    }).Where(b => b.X < tp.X).OrderBy(b => b.X).LastOrDefault();
                                    i = pickedKeys.Children.IndexOf(t.b);
                                }
                            } else {
                                i = -1;
                            }
                        } else if (ing = v.Length > width / 3) {
                            m.Margin = new Thickness {
                                Left = p.X,
                                Top = p.Y
                            };
                            Children.Add(m);
                        }
                    };
                    TouchLeave += leave = (s, e) => {
                        if (de.Device != e.Device) {
                            return;
                        }
                        ing = false;
                        TouchMove -= move;
                        TouchLeave -= leave;
                        item.IsHitTestVisible = true;
                        if (i > -1) {
                            var ev = true;
                            var n = new SquareButton(false) {
                                Tag = item.Tag,
                                Content = item.Content
                            };//new button
                            n.EffectOn();
                            n.TouchDown += delegate {
                                n.ToggleEffect(ev = !ev);
                            };
                            pickedKeys.Children.Insert(i, n);
                        }
                        Children.Remove(m);
                    };
                };
            });
            var traces = new List<Trace>();
            var effects = new List<TouchEffect>();
            Action<Trace> add = trace => {
                if (traces.Count(t => t.Released) == traces.Count(t => !t.Released)) {
                    effects.ForEach(e => e.Reset());
                }
                traces.Add(trace);
            };
            foreach (var item in Beard.RawPoints) {
                var te = new TouchEffect(tracesBoard, new Trace { Center = item, Radius = Beard.Radius }, 0);
                te.ReadyToMove += add;
                te.StopMoving += add;
                effects.Add(te);
            }
            TouchDown += (s, e) => {
                var eq = e.Source == allKeysBoard ? 1 : e.Source == tracesBoard ? 2 : 0;
                if (eq > 0) {
                    tracesBoard.IsHitTestVisible = eq == 1;
                    tracesBoard.Opacity = eq == 1 ? 1 : .1;
                }
            };
            pickedKeysScroller.RequestBringIntoView += (s, e) => e.Handled = true;
            EventHandler<TouchEventArgs> pickedKeysDown = null;
            pickedKeys.TouchDown += pickedKeysDown = (s, de) => {
                pickedKeys.TouchDown -= pickedKeysDown;
                var x = de.GetTouchPoint(this).Position.X;
                var d = pickedKeysScroller.HorizontalOffset;
                EventHandler<TouchEventArgs> leave = null, move = null;
                TouchLeave += leave = (ss, le) => {
                    if (le.Device == de.Device) {
                        TouchLeave -= leave;
                        TouchMove -= move;
                        pickedKeys.TouchDown += pickedKeysDown;
                    }
                };
                TouchMove += move = (ss, me) => {
                    if (me.Device == de.Device) {
                        pickedKeysScroller.ScrollToHorizontalOffset(d + x - de.GetTouchPoint(this).Position.X);
                    }
                };
            };
        }
    }
}
