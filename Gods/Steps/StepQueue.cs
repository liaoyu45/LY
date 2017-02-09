using System;
using System.Collections;
using System.Collections.Generic;

namespace Gods.Steps {
    public class StepQueue : IEnumerable<Step> {

        public int Progress { get; private set; } = -1;

        private List<Lazy<Step>> steps = new List<Lazy<Step>>();

        public void Add<T>() where T : Step, new() {
            steps.Add(new Lazy<Step>(() => new T()));
        }

        public void Remove<T>() {
            var t = typeof(T);
            steps.Remove(steps.Find(s => s.GetType().GenericTypeArguments[0] == t));
        }

        private Step current() {
            if (Progress > -1 && Progress < steps.Count) {
                return steps[Progress].Value;
            }
            return null;
        }

        public void Start() {
            Progress = 0;
            current()?.Init(1);
        }

        public bool Move(int offset) {
            var old = current();
            bool? canMove;
            if (offset > 0) {
                canMove = old?.Finish();
            } else if (offset < 0) {
                canMove = old?.Cancel();
            } else {
                canMove = true;
            }
            if (!canMove.HasValue || !canMove.Value) {
                return false;
            }
            Progress += offset;
            var s = current();
            s?.Init(offset);
            return s != null;
        }

        public IEnumerator<Step> GetEnumerator() {
            foreach (var item in steps) {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
