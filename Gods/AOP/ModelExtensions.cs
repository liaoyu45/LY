using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.AOP {
    public static class ModelExtensions {
        private static Dictionary<Guid, List<IValidator>> validators = new Dictionary<Guid, List<IValidator>>();

        public static void AddValidator<T>(IValidator validator) where T : ModelBase {
            var guid = typeof(T).GUID;
            if (validators[guid] == null) {
                validators[guid] = new List<IValidator>();
            }
            validators[guid].Add(validator);
        }

        public static IEnumerable<IValidator> GetValidators(Type type) {
            var vs = validators[type.GUID];//TODO: retest all this system.
            if (vs?.Count > 0) {
                return vs.ToArray();
            }
            var all = Him.GetBases(type, typeof(ModelBase));
            foreach (var g in all) {
                var vsvs = validators[g.GUID];
                if (vsvs?.Any(v => v.Inheritable) == true) {
                    return vsvs;
                }
            }
            return null;
        }
    }
}