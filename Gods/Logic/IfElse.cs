using System;

namespace Gods.Logic {
    public class IfElse {
        private bool expect;

        public Func<bool> Condition { private get; set; }
        public Action IfTrue { private get; set; }
        public Action IfFalse { private get; set; }

        /// <summary>
        /// 每次获取的结果极有可能不一样。
        /// </summary>
        public bool Assert(IfElseResult result) {
            IfElseResult r;
            try {
                var v = Condition?.Invoke() == expect;
                if (v == true) {
                    try {
                        IfTrue?.Invoke();
                        r = IfElseResult.TC0;
                    } catch {
                        r = IfElseResult.TC1;
                    }
                } else {
                    try {
                        IfFalse?.Invoke();
                        r = IfElseResult.FC0;
                    } catch {
                        r = IfElseResult.FC1;
                    }
                }
            } catch {
                r = IfElseResult.UC;
            }
            return (r | result) == r;
        }

        public IfElse() : this(true) { }

        public IfElse(bool expect) {
            this.expect = expect;
        }
    }

    /// <summary>
    /// <see cref="IfElse"/>的执行状态。
    /// </summary>
    public enum IfElseResult {
        /// <summary>
        /// <see cref="IfElse.Condition"/>发生异常。
        /// </summary>
        UC = 0,
        /// <summary>
        /// <see cref="IfElse.Condition"/>返回 true。
        /// </summary>
        C0 = 1,
        /// <summary>
        /// <see cref="IfElse.Condition"/>返回 false。
        /// </summary>
        C1 = 2,
        /// <summary>
        /// <see cref="IfElse.IfTrue"/>成功执行。
        /// </summary>
        T0 = 4,
        /// <summary>
        /// <see cref="IfElse.IfTrue"/>发生异常。
        /// </summary>
        T1 = 8,
        /// <summary>
        /// <see cref="IfElse.IfFalse"/>成功执行。
        /// </summary>
        F0 = 16,
        /// <summary>
        /// <see cref="IfElse.IfFalse"/>发生异常。
        /// </summary>
        F1 = 32,
        TC0 = C0 | T0, TC1 = C0 | T1, FC0 = C1 | F0, FC1 = C1 | F1
    }
}
