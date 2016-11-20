using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Siblings {
    public abstract partial class SiblingsContext<M> : ISiblingsContext<M> where M : Model<M> {
        List<M> all = new List<M>();

        public virtual bool NoticeAllWhenLeave { get; }

        public IEnumerator<M> GetEnumerator() {
            return this.all.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        public void Add(M model) {
            model.Context = this;
            model.records = new Dictionary<int, bool>();

            var ids = this.Select(m => m.id);
            var goneIds = Enumerable.Range(0, ids.Max()).Except(ids);
            model.id = goneIds.Any() ? goneIds.First() : this.all.Count;
            foreach (var s in this) {
                model.records[s.id] = model.CanSee(s, When.SelfJoined);
                var addToo = s.CanSee(model, When.SiblingJoined);
                s.records[model.id] = addToo;
                if (addToo) {
                    s.OnSiblingJoined(model);
                }
            }
            this.all.Add(model);
            model.ReLoaded();
        }
    }
}