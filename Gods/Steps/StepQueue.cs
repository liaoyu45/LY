using System;
using System.Collections.Generic;

namespace Gods.Steps {
    /// <summary>
    /// 执行序列，分步并且单步执行。
    /// </summary>
    /// <typeparam name="T">各步共用的目标类型。</typeparam>
    /// <typeparam name="S">步骤类型。</typeparam>
    public class StepQueue<T, S> where T : class, new() where S : Step<T> {
        public int progress { get; private set; } = -1;
        public T target { get; private set; }

        public List<S> Steps { get; } = new List<S>();
        public bool Ongoing { get; private set; }

        public void Start(T target) {
            this.target = target;
            Move(true);
        }

        public void Move(bool direction) {
            foreach (var item in Steps) {
                item.Target = target;
            }
            var c = Steps[progress];
            if (direction) {
                c.Finish();
                if (progress < Steps.Count - 1) {
                    progress++;
                    Steps[progress].Initiate(c.Result, true);
                }
            } else {
                c.Cancel();
                if (progress > 0) {
                    progress--;
                    Steps[progress].Initiate(null, false);
                }
            }
        }
    }
}
