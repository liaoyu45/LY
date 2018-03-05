namespace Gods.Steps {
    public abstract class Step {

        internal protected virtual int Finish() => 1;
        internal protected virtual int Cancel() => -1;

        internal protected abstract void Init(int offset);
        protected internal virtual bool WillRecreate { get; set; }
    }
}
