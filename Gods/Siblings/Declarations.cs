using System.Collections.Generic;

namespace Gods.Siblings {
    partial interface IModel<M, out C> where M : IModel<M, ISiblingsContext<M>> where C : ISiblingsContext<M> {
        bool CanSee(M sibling, When state);
        C Context { get; }
    }

    partial interface ISiblingsContext<M> : IEnumerable<M> where M : IModel<M, ISiblingsContext<M>> { }

    public enum When {
        SelfJoined = 1,
        SiblingJoined = 2,
        SelfIsRemoved = 4,
        SiblingLeft = 8,
        Other = 16
    }
}
