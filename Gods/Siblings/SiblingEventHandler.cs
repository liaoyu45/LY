namespace Gods.Siblings {
    public delegate void SiblingEventHandler<M>(M self, M sibling) where M : Model<M>;
}
