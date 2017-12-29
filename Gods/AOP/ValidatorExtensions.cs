using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.AOP {
	public static class ValidatorExtensions {
		private static Dictionary<Guid, List<IValidator>> all = new Dictionary<Guid, List<IValidator>>();

		public static void Validate<T>(this IValidator validator) where T : ModelBase {
			var guid = typeof(T).GUID;
			if (!all.ContainsKey(guid)) {
				all[guid] = new List<IValidator>();
			}
			var vt = validator.GetType();
			if (all[guid].All(e => e.GetType() != vt)) {
				all[guid].Add(validator);
			}
		}

		internal static IEnumerable<IValidator> GetValidators(Type type) {
			if (!all.ContainsKey(type.GUID)) {
				return Enumerable.Empty<IValidator>();
			}
			var vs = all[type.GUID];
			if (vs.Count > 0) {
				return vs.ToArray();
			}
			return Him.GetBases(type, typeof(ModelBase))
				.Where(t => all.ContainsKey(t.GUID))
				.SelectMany(t => all[t.GUID])
				.Where(v => v.Inheritable);
		}
	}
}