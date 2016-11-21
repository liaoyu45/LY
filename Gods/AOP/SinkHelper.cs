using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods.AOP {
    class SinkHelper {
        internal SinkHelper(ModelBase m) {
            this.model = m;
            this.type = m.GetType();
        }

        private ModelBase model;
        private Type type;

        static bool inMode(IValidator v, Modes m) {
            return (v.Mode & m) == m;
        }

        internal void Analysis(MethodBase calling) {
            var vs = ModelExtensions.GetValidators(this.type);
            foreach (var v in vs) {
                if ((Modes.Foreigner | Modes.Sibling | v.Mode) != (Modes.Foreigner | Modes.Sibling)) {
                    return;
                }
                var callers = Him.GetCallers(this.type);
                var badCallers = new List<MethodBase>();
                if (inMode(v, Modes.Sibling)) {
                    var siblings = callers.Where(isSibling).Where(c => !v.Sibling(this.model, calling, c));
                    badCallers.AddRange(siblings);
                }
                if (inMode(v, Modes.Foreigner)) {
                    var foreigners = callers.Where(isForeigner).Where(c => !v.Foreigner(this.model, calling, c));
                    badCallers.AddRange(foreigners);
                }
                if (badCallers.Any()) {
                    throw new AOPException(this.model, calling, badCallers.ToArray());
                } 
            }
        }

        private bool isSibling(MethodBase c) {
            return c.DeclaringType.BaseType == this.type.BaseType;
        }

        private bool isForeigner(MethodBase c) {
            return c.DeclaringType.BaseType != this.type.BaseType;
        }
    }
}