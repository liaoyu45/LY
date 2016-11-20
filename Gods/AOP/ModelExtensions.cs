using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.AOP {
    public static class ModelExtensions {
        private static Dictionary<Guid, List<IValidator>> validators = new Dictionary<Guid, List<IValidator>>();

        public static void AddValidator(this ModelBase model, IValidator validator) {
            var guid = model.GetType().GUID;
            if (validators.ContainsKey(guid)) {
                validators[guid].Add(validator);
            } else {
                var list = new List<IValidator>();
                list.Add(validator);
                validators.Add(guid, list);
            }
        }

        public static IEnumerable<IValidator> GetValidator(Type type) {
            if (!type.IsSubclassOf(typeof(ModelBase))) {
                return null;
            }
            var vs = validators[type.GUID];
            if (vs?.Count > 0) {
                return vs;//TODO, ((List<T>)vs).Add(t), will change?
            }
            var all = type.GUIDTill(typeof(ModelBase));
            foreach (var g in all) {
                if (validators.ContainsKey(g)) {
                    var vsvs = validators[g];
                    if (vsvs?.Any(v => v.Inheritable) == true) {
                        return vsvs;
                    }
                }
            }
            return null;
        }
    }
}