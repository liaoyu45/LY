using System.Collections.Generic;
using System.Linq;

namespace Gods.Siblings {
    public abstract class Model<M> : IModel<M, SiblingsContext<M>> where M : Model<M> {
        public SiblingsContext<M> Context { get; internal set; }
        internal int id;
        internal Dictionary<int, bool> records;
        private M self => this as M;

        public IEnumerable<M> InvisibleSiblings => this.records.Where(r => !r.Value).Select(r => this.Context.First(m => m.id == r.Key));

        public IEnumerable<M> VisibleSiblings => this.records.Where(r => r.Value).Select(r => this.Context.First(m => m.id == r.Key));

        public virtual bool CanSee(M sibling, When when) {
            return true;
        }

        public bool CanSee(M sibling) {
            return sibling != null && sibling.Context == this.Context && this.records.ContainsKey(sibling.id) && this.records[sibling.id];
        }

        internal void OnSiblingJoined(M sibling) {
            this.SiblingJoined?.BeginInvoke(this.self, sibling, null, null);
        }

        internal void ReLoaded() {
            this.Loaded?.BeginInvoke(this.self, null, null, null);
        }

        protected virtual bool AutoRemove(M sibling) {
            return true;
        }

        public void Remove(M sibling) {
            if (!CanSee(this.self, sibling, When.SelfJoined)) {
                return;
            }
            this.Removed?.BeginInvoke(this.self, sibling, null, null);
            this.records[sibling.id] = false;
            if (sibling.CanSee(this.self, When.SelfIsRemoved)) {
                sibling.SelfIsRemoved?.BeginInvoke(sibling, this.self, null, null);
            }
            if (sibling.AutoRemove(this.self)) {
                sibling.Remove(this.self);
            }
        }

        public static bool CanSee(M self, M sibling, When when) {
            if (self == null || self.Context == null || sibling == null || self == sibling || self.Context != sibling.Context) {
                return false;
            }
            return self.CanSee(sibling, when);
        }

        public void Leave() {
            if (this.Context == null) {
                return;
            }
            foreach (var s in this.Context.NoticeAllWhenLeave ? this.Context : this.VisibleSiblings) {
                if (s.CanSee(this.self, When.SiblingLeft)) {
                    s.SiblingLeft?.BeginInvoke(s, this.self, null, null);
                }
                s.records.Remove(this.id);
            }
            this.Left?.BeginInvoke(this.self, null, null, null);
            this.Context = null;
            this.records.Clear();
        }

        public event SiblingEventHandler<M> Left;
        public event SiblingEventHandler<M> Loaded;
        public event SiblingEventHandler<M> Removed;
        public event SiblingEventHandler<M> SelfIsRemoved;
        public event SiblingEventHandler<M> SiblingJoined;
        public event SiblingEventHandler<M> SiblingLeft;
    }
}