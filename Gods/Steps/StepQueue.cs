using System;
using System.Collections.Generic;

namespace Gods.Steps {
    public abstract class StepQueue {
        private int progress;

        public List<Step> Steps { get; } = new List<Step>();
        public StepQueue(Step one, Step two, params Step[] more) {
            Steps.Add(one);
            Steps.Add(two);
            Steps.AddRange(more);
            doo(0, true);
        }

        public void Stop() {
            Steps[progress].Finished = null;
            Steps[progress].Canceled = null;
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void doo(int i, bool d) {
            Steps[i].Finished = doAndClear(s => {
                progress = Steps.IndexOf(s) + 1;
                if (progress < Steps.Count) {
                    doo(progress, true);
                } else {
                    Finished?.Invoke(this, EventArgs.Empty);
                }
            });
            Steps[i].Canceled = doAndClear(s => {
                progress = Steps.IndexOf(s) - 1;
                if (progress > -1) {
                    doo(progress, false);
                } else {
                    Canceled?.Invoke(this, EventArgs.Empty);
                }
            });
            Steps[i].Execute(d);
        }

        private Action<Step> doAndClear(Action<Step> a) {
            return s => {
                a(s);
                s.Finished = null;
                s.Canceled = null;
            };
        }

        public event EventHandler Finished;
        public event EventHandler Canceled;
    }
}
