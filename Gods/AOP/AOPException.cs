using System;
using System.Reflection;

namespace Gods.AOP {
    public class AOPException : Exception {
        public ModelBase Target { get; }
        public MethodBase[] BadCallers { get; }
        public MethodBase Calling { get; }

        public AOPException(ModelBase target, MethodBase calling, params MethodBase[] badCallers) {
            this.Target = target;
            this.Calling = calling;
            this.BadCallers = badCallers;
        }
    }
}