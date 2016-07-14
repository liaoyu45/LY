using System.Collections;
using System.Collections.Generic;

namespace KeyboardSynchronizer {
    public class InfiniteEnumerator<T> : IEnumerator<T> {
        List<T> list;
        int startIndex;

        public int Index { get; private set; }
        public int Step { get; set; } = 1;
        public InfiniteEnumerator(List<T> source) {
            list = source;
        }

        public T Current {
            get { return list[Index]; }
        }

        object IEnumerator.Current {
            get { return Current; }
        }

        public void Dispose() {
        }

        public bool MoveNext() {
            if (list.Count == 0) {
                return false;
            }
            if (Step == 0) {
                return false;
            }
            Index += Step;
            while (Index > list.Count - 1) {
                Index -= list.Count;
            }
            while (Index < 0) {
                Index += list.Count;
            }
            return true;
        }

        public void Reset() {
            Index = startIndex;
        }

        public void Reset(int start, int step) {
            Index = start - step;
            startIndex = start;
            Step = step;
        }
    }
}