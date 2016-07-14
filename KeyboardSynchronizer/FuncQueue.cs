using System.Collections.Generic;

namespace KeyboardSynchronizer {
    public abstract class FuncQueue<T> {
        public bool Adding { get; set; }
        public T IdToggler { get; set; }
        public T Starter { get; set; }
        public int Id { get; set; } = -1;
        public int Index { get; set; } = -1;
        public List<T> Parameters { get; } = new List<T>();
        public abstract T Empty { get; }
        public T[] Restarter { get; set; }

        protected abstract bool IsIndexElement(T t);
        protected abstract int CreateNewIndex(T t);
        protected abstract bool Equals(T a, T b);

        public void Insert(T t) {
            if (Equals(t, Empty)) {
                return;
            }
            if (Adding) {
                add(t);
            } else if (IsIndexElement(t)) {
                Index = CreateNewIndex(t);
            } else if (Equals(IdToggler, Empty)) {
                IdToggler = t;
                Id = 0;
            } else if (Equals(IdToggler, t)) {
                if (Index == -1) {
                    Id++;
                } else {
                    reset();
                }
            } else {
                Starter = t;
                Adding = true;
            }
        }

        void reset() {
            Id = -1;
            Index = -1;
            Starter = Empty;
            Adding = false;
            Parameters.Clear();
        }

        void add(T t) {
            if (Restarter == null || Restarter.Length == 0 || Parameters.Count + 1 < Restarter.Length || Equals(Restarter[Restarter.Length - 1], t)) {
                Parameters.Add(t);
                return;
            }
            var i = Parameters.Count - 1;
            var j = Restarter.Length - 2;
            while (i >= 0 && j >= 0) {
                if (Equals(Parameters[i], Restarter[j])) {
                    i--;
                    j--;
                    continue;
                }
                Parameters.Add(t);
                return;
            }
            reset();
        }
    }
}
