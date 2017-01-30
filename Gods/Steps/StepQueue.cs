using System.Collections.Generic;

namespace Gods.Steps {
    public class StepQueue {

        public int Progress { get; private set; } = -1;

        public List<Step> Steps { get; } = new List<Step>();

        private Step current() {
            if (Progress > -1 && Progress < Steps.Count) {
                return Steps[Progress];
            }
            return null;
        }

        public void Start() {
            Progress = 0;
            current()?.Init(1);
        }

        public bool Move(int offset) {
            var old = current();
            bool? canMove;
            if (offset > 0) {
                canMove = old?.Finish();
            } else if (offset < 0) {
                canMove = old?.Cancel();
            } else {
                canMove = true;
            }
            if (!canMove.HasValue || !canMove.Value) {
                return false;
            }
            Progress += offset;
            var s = current();
            s?.Init(offset);
            return s != null;
        }
    }
}
