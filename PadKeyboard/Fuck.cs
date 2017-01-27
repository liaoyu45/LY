using System;
using System.Reflection;

namespace PadKeyboard {
    class Fuck {
        public class Test {
            public event EventHandler AA;
            public void Foo() {
                AA?.Invoke(this, new EventArgs());
            }
        }

        static void Main11(string[] args) {
            Test obj = new Test();
            obj.AA += delegate { Console.WriteLine("event raised."); };
            obj.Foo();
            RemoveEvent(obj, "AA");
            obj.Foo();
            Console.ReadKey();
        }

        static void RemoveEvent<T>(T c, string name) {
            Delegate[] invokeList = GetObjectEventList(c, "AA");
            foreach (Delegate del in invokeList) {
                typeof(T).GetEvent("AA").RemoveEventHandler(c, del);
            }
        }

        public static Delegate[] GetObjectEventList(object p_Object, string p_EventName) {
            FieldInfo _Field = p_Object.GetType().GetField(p_EventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (_Field == null) {
                return null;
            }
            object _FieldValue = _Field.GetValue(p_Object);
            if (_FieldValue != null && _FieldValue is Delegate) {
                Delegate _ObjectDelegate = (Delegate)_FieldValue;
                return _ObjectDelegate.GetInvocationList();
            }
            return null;
        }



    }
}