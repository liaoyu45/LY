using System.Collections.Generic;
using System.Linq;

namespace Gods.Siblings {
    public abstract class Model<M> : IModel<M, SiblingsContext<M>> where M : Model<M> {

        public SiblingsContext<M> Context { get; internal set; }

        internal int id;
        internal Dictionary<int, bool> records;
        private M self => this as M;

        public IEnumerable<M> Siblings => Context.all.Where(m => m.id != id);

        public IEnumerable<M> InvisibleSiblings => records.Where(r => !r.Value).Select(r => Context.all.First(m => m.id == r.Key));

        public IEnumerable<M> VisibleSiblings => records.Where(r => r.Value).Select(r => Context.all.First(m => m.id == r.Key));

        protected virtual bool AutoRemove(M sibling) => true;

        protected internal virtual bool CanSee(M sibling, When when) => true;

        public bool CanSee(M sibling) =>
            sibling?.Context == Context && records[sibling.id];

        internal void OnSiblingJoined(M sibling) =>
            SiblingJoined?.BeginInvoke(self, sibling, null, null);

        /// <summary>
        /// Make a sibling invisible.
        /// </summary>
        public void Remove(M sibling) {
            if (!records[sibling.id]) {
                return;
            }
            records[sibling.id] = false;
            if (sibling.CanSee(self, When.SelfRemoved)) {
                sibling.SelfRemoved?.BeginInvoke(sibling, self, null, null);
            }
            if (sibling.AutoRemove(self)) {
                sibling.Remove(self);
            }
        }


        /// <summary>
        /// Remove self from Context.
        /// </summary>
        public void Leave() {
            if (Context == null) {
                return;
            }
            foreach (var s in Context.all) {
                if (s.CanSee(self, When.SiblingLeft)) {
                    s.SiblingLeft?.BeginInvoke(s, self, null, null);
                }
                s.records.Remove(id);
            }
            Context.all.Remove(self);
            Context = null;
            records.Clear();
        }

        /// <summary>
        /// When self becomes invisible to a sibling.
        /// </summary>
        public event SiblingEventHandler<M> SelfRemoved;
        public event SiblingEventHandler<M> SiblingJoined;
        public event SiblingEventHandler<M> SiblingLeft;
    }
}