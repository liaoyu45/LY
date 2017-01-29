using System;

namespace Gods.Steps {
    public abstract class Step<T> : IComparable<Step<T>> where T : class {

        internal protected virtual void Finish() { }
        internal protected virtual void Cancel() { }

        internal bool isNew;

        internal protected T Target { get; internal set; }
        internal protected object Result { get; set; }

        internal protected abstract int Index { get; set; }
        internal protected abstract void Initiate(object result, bool direction);

        public int CompareTo(Step<T> other) {
            return Index - other.Index;
        }
    }
}
