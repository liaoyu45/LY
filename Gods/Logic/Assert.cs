using System;

namespace Gods.Logic {

    public class Assert {
        private bool expect;

        public bool? Check { get; private set; }
        public bool? Fail { get; private set; }
        public bool? Success { get; private set; }

        public Func<bool> OnCheck { private get; set; }
        public Action OnSuccess { private get; set; }
        public Action OnFail { private get; set; }

        private bool? completed;

        internal bool Completed {
            get {
                if (completed.HasValue) {
                    return completed.Value;
                }
                try {
                    Check = OnCheck?.Invoke() == expect;
                    if (Check == true) {
                        try {
                            OnSuccess?.Invoke();
                            Success = true;
                        } catch {
                            Success = false;
                        }
                    } else {
                        try {
                            OnFail?.Invoke();
                            Fail = true;
                        } catch {
                            Fail = false;
                        }
                    }
                } catch {
                }
                completed = Check == true && Success == true;
                return Completed;
            }
        }

        public Assert() : this(true) {
        }

        public Assert(bool expect) {
            this.expect = expect;
        }
    }
}
