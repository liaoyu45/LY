namespace Gods.Steps {
    public abstract class Step {

        internal protected virtual bool Finish() => true;
        internal protected virtual bool Cancel() => true;

        internal protected abstract void Init(int offset);
    }
}
