using System.Collections.Generic;

namespace Gods.Siblings {
    partial interface IModel<M, out C> where M : IModel<M, ISiblingsContext<M>> where C : ISiblingsContext<M> {
    }

    partial interface ISiblingsContext<M> where M : IModel<M, ISiblingsContext<M>> {
    }

    public enum When {
        /// <summary>
        /// When joins a context.
        /// </summary>
        Join = 1,
        /// <summary>
        /// When a new memeber joins current context.
        /// </summary>
        NewMember = 2,
        /// <summary>
        /// When becomes invisible to a sibling.
        /// </summary>
        SelfRemoved = 4,
        /// <summary>
        /// When a sibling leaves.
        /// </summary>
        SiblingLeft = 8,
        Other = 16
    }
}
