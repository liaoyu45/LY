using System;

namespace Gods.Steps {
    public abstract class Step {

        internal Action<Step> Finished;
        internal Action<Step> Canceled;
        protected void Finish() {
            Finished?.Invoke(this);
        }
        protected void Cancel() {
            Canceled?.Invoke(this);
        }
        internal protected abstract void Execute(bool direction);
    }
}
