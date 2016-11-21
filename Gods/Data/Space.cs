using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Data {
    class Space : IEnumerable<Space> {
        public object Filler {
            get {
                return filler ?? (filler = Top.create());
            }
        }
        public Space Upper { get; private set; }
        public Space Top => Upper?.Top ?? this;

        protected Func<object> create;
        protected object filler;
        public int[] Chain { get; set; }
        public int[] Dementions { get; }
        Space[] all;

        static Random r { get; } = new Random();

        public Space(int i, params int[] indexes) : this(() => r.Next(2), i, indexes) { }

        public Space(Func<object> create, int i, params int[] indexes) : this(new int[0], new[] { i }.Concat(indexes).ToArray()) {
            this.create = create;
        }

        private Space(int[] chain, params int[] indexes) {
            Chain = chain;
            Dementions = indexes;
            var i = indexes[0];
            all = new Space[i];
            var chains = Enumerable.Range(0, i).Select(e => chain.Concat(new int[] { e }).ToArray()).ToArray();
            if (indexes.Length == 1) {
                while (i-- > 0) {
                    all[i] = new Space(chains[i]);
                    all[i].Upper = this;
                }
            } else {
                var _indexes = indexes.Skip(1).ToArray();
                while (i-- > 0) {
                    all[i] = new Space(chains[i], _indexes);
                    all[i].Upper = this;
                }
            }
            filler = all;
        }

        private Space(int[] chain) {
            Chain = chain;
        }

        public Space this[int i, params int[] indexes] {
            get {
                if (all == null) {
                    return (Filler as Space)?[i, indexes];
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

        public IEnumerator<Space> GetEnumerator() {
            return ((IEnumerable<Space>)all).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<Space>)all).GetEnumerator();
        }
    }
}