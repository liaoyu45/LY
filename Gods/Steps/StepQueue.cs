using System;
using System.Collections.Generic;

namespace Gods.Steps {
    public class StepQueue {

        public int Progress { get; private set; } = -1;

        public List<Lazy<Step>> Steps { get; } = new List<Lazy<Step>>();

        public void Add<T>() where T : Step, new() {
            Steps.Add(new Lazy<Step>(() => new T()));
        }

        public Step Current {
            get {
                if (Progress > -1 && Progress < Steps.Count) {
                    return Steps[Progress].Value;
                }
                return null;
            }
        }

        public void Start() {
            Progress = 0;
            Current?.Init(1);
        }

        public void Move(bool d) {
            var s = d ? Current.Finish() : Current.Cancel();
            if (s == 0) {
                return;
            }
            Progress += s;
            if (Progress < 0) {
                Progress = 0;
                Cancelled?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (Progress > Steps.Count - 1) {
                Progress = Steps.Count - 1;
                Finished?.Invoke(this, EventArgs.Empty);
                return;
            }
            Current.Init(s);
            if (Current.WillRecreate) {
                var type = Current.GetType();
                Steps[Progress] = new Lazy<Step>(() => (Step)Activator.CreateInstance(type));
                Current.Init(s);
            }
        }

        public event EventHandler Finished, Cancelled;
    }
}
