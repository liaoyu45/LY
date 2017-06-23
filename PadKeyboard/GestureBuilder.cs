//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;

//namespace PadKeyboard {
//    class GestureBuilder {
//        public List<Trace> List { get; set; } = new List<Trace>();

//        public void Leave(int index, Vector v) {
//            if (List == null || List.LastOrDefault(i => i.Index == index)?.Released != false) {
//                return;
//            }
//            List.Add(new Trace { Index = index, Direction = v, Released = true });
//            if (List.GroupBy(i => i.Index).All(i => i.Last().Released)) {
//                Finished?.BeginInvoke(this, EventArgs.Empty, delegate {
//                    List = null;
//                }, null);
//            }
//        }

//        public void Down(int index) {
//            if (List == null || List.LastOrDefault(i => i.Index == index)?.Released == false) {
//                return;
//            }
//            if (List.LastOrDefault(i => i.Index == index)?.Released != false) {
//                List.Add(new Trace { Index = index });
//            }
//        }
//    }
//}