using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Siblings {
    public abstract partial class SiblingsContext<M> : ISiblingsContext<M> where M : Model<M> {
        internal List<M> all = new List<M>();

        public IEnumerator<M> GetEnumerator() {
            return all.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public void Add(M model) {
            if (model.Context == this || model.UniqueContext && model.Context != null ) {
                return;
            }
            model.Context = this;
            model.records = new Dictionary<int, bool>();

            var ids = all.Select(m => m.id);
            var goneIds = Enumerable.Range(0, ids.Max()).Except(ids);
            model.id = goneIds.Any() ? goneIds.First() : all.Count;
            foreach (var s in all) {
                model.records[s.id] = model.CanSee(s, When.Join);
                var addToo = s.CanSee(model, When.NewMember);
                s.records[model.id] = addToo;
                if (addToo) {
                    s.OnSiblingJoined(model);
                }
            }
            all.Add(model);
        }
    }
}