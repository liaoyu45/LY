using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Data {
    class Space<T> : IEnumerable<Space<T>> {
        public T Filler {
            get {
                if (!ed) {
                    ed = true;
                    return (filler = Top.create());
                }
                return filler;
            }
        }
        public Space<T> Upper { get; private set; }
        public Space<T> Top => Upper?.Top ?? this;

        private bool ed;
        private Func<T> create;
        private T filler;
        public int[] Chain { get; set; }
        public int[] Dementions { get; }
        Space<T>[] all;

        public Space(Func<T> create, int i, params int[] indexes) : this(new int[0], new[] { i }.Concat(indexes).ToArray()) {
            this.create = create;
        }

        private Space(int[] chain, params int[] indexes) {
            Chain = chain;
            Dementions = indexes;
            var i = indexes[0];
            all = new Space<T>[i];
            var chains = Enumerable.Range(0, i).Select(e => chain.Concat(new int[] { e }).ToArray()).ToArray();
            if (indexes.Length == 1) {
                while (i-- > 0) {
                    all[i] = new Space<T>(chains[i]);
                    all[i].Upper = this;
                }
            } else {
                var _indexes = indexes.Skip(1).ToArray();
                while (i-- > 0) {
                    all[i] = new Space<T>(chains[i], _indexes);
                    all[i].Upper = this;
                }
            }
        }

        private Space(int[] chain) {
            Chain = chain;
        }

        public Space<T> this[int i, params int[] indexes] {
            get {
                if (all == null) {
                    return (Filler as Space<T>)?[i, indexes];
                }
                if (indexes.Length == 0) {
                    return all[i];
                }
                if (indexes.Length == 1) {
                    return all[i][indexes[0]];
                }
                return all[i][indexes[0], indexes.Skip(1).ToArray()];
            }
        }

        public override string ToString() {
            return all?.Aggregate(string.Empty, (a, s) => a + s + ',').Trim(',') ?? Filler.ToString();
        }

        public override bool Equals(object obj) {
            if (GetType() != obj?.GetType()) {
                return false;
            }
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode() {
            if (Upper == null) {
                return base.GetHashCode();
            }
            return Top.GetHashCode() - Chain.GetHashCode();
        }

        public IEnumerator<Space<T>> GetEnumerator() {
            return ((IEnumerable<Space<T>>)all).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<Space<T>>)all).GetEnumerator();
        }
    }
}