using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.AOP {
    public static class ModelExtensions {
        private static Dictionary<Guid, List<IValidator>> validators = new Dictionary<Guid, List<IValidator>>();

        public static void AddValidator(Type type, IValidator validator) {
            var guid = type.GUID;
            if (validators[guid] == null) {
                validators[guid] = new List<IValidator>();
            }
            validators[guid].Add(validator);
        }

        public static IEnumerable<IValidator> GetValidators(Type type) {
            var vs = validators[type.GUID];
            if (vs?.Count > 0) {
                return vs.ToArray();
            }
            return Him.GetBases(type, typeof(ModelBase))
                .Select(b => validators[b.GUID]).Where(vsvs => vsvs != null)
                .SelectMany(v => v).Where(v => v.Inheritable);
        }
    }
}