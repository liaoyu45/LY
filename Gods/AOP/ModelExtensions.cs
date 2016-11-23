using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.AOP {
    public static class ModelExtensions {
        private static Dictionary<Guid, List<IValidator>> all = new Dictionary<Guid, List<IValidator>>();

        public static void AddValidator(Type type, IValidator validator) {
            var guid = type.GUID;
            if (!all.ContainsKey(guid)) {
                all[guid] = new List<IValidator>();
            }
            all[guid].Add(validator);
        }

        public static IEnumerable<IValidator> GetValidators(Type type) {
            var vs = all[type.GUID];
            if (vs?.Count > 0) {
                return vs.ToArray();
            }
            return Him.GetBases(type, typeof(ModelBase)).Where(t => all.ContainsKey(t.GUID))
                .SelectMany(b => all[b.GUID]).Where(v => v.Inheritable);
        }
    }
}