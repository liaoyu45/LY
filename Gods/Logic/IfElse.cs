using System;

namespace Gods.Logic {
    public class IfElse {
        /// <summary>
        /// 预期的 <see cref="Condition"/> 的返回值。
        /// </summary>
        public bool Expect { private get; set; } = true;
        /// <summary>
        /// 当 <see cref="IfError"/> 为 null 时，是否使用 <see cref="IfFalse"/> 代替。
        /// </summary>
        public bool IfFalseAsIfError { private get; set; } = true;
        /// <summary>
        /// 如果返回 true，则将执行 <see cref="IfTrue"/>，否则执行 <see cref="IfFalse"/>，发生异常则执行 <see cref="IfError"/>。
        /// </summary>
        public Func<bool> Condition { private get; set; }
        /// <summary>
        /// 当 <see cref="Condition"/> 返回 true 时。
        /// </summary>
        public Action IfTrue { private get; set; }
        /// <summary>
        /// 当 <see cref="Condition"/> 返回 false 时。
        /// </summary>
        public Action IfFalse { private get; set; }
        /// <summary>
        /// 当执行 <see cref="Condition"/> 时发生异常时。
        /// </summary>
        public Action IfError { private get; set; }

        /// <summary>
        /// 每次获取的结果极有可能不一样。
        /// </summary>
        public bool Assert(IfElseResult result) {
            IfElseResult r;
            try {
                var v = Condition?.Invoke() == Expect;
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
                try {
                    if (IfError == null) {
                        if (IfFalseAsIfError) {
                            IfFalse?.Invoke();
                        }
                    } else {
                        IfError?.Invoke();
                    }
                    r = IfElseResult.E0;
                } catch {
                    r = IfElseResult.E1;
                }
            }
            return (r | result) == r;
        }
    }

    /// <summary>
    /// <see cref="IfElse"/>的执行状态。
    /// </summary>
    public enum IfElseResult {
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
        /// <summary>
        /// <see cref="IfElse.IfError"/>成功执行。
        /// </summary>
        E0 = 64,
        /// <summary>
        /// <see cref="IfElse.IfError"/>发生异常。
        /// </summary>
        E1 = 128,
        TC0 = C0 | T0, TC1 = C0 | T1, FC0 = C1 | F0, FC1 = C1 | F1, E = E0 | E1
    }
}
