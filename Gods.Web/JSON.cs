using System;
using System.Reflection;

namespace Gods.Web {

	public class JSON {
		private const BindingFlags allps = (BindingFlags)63;

		private JSON(object source) {
			if (source == null) {
				throw new ArgumentException(nameof(source));
			}
			Source = source;
		}

		public static JSON From(object source) {
			return source == null ? null : new JSON(source);
		}

		public object Source { get; private set; }

		public void CopyTo(object target, params string[] excludes) {
			CopyTo(target, true, excludes);
		}
		public void CopyTo(object target, bool contains, params string[] props) {
			Gods.Him.CopyTo(Source, target, contains, props);
		}
		public T CopyTo<T>(bool contains, params string[] props) where T : new() {
			return Gods.Him.CopyTo<T>(Source, contains, props);
		}

		public T CopyTo<T>(params string[] excludes) where T : new() {
			return CopyTo<T>(true, excludes);
		}
		public object this[string name] {
			get {
				return Source.GetType().GetProperty(name, allps)?.GetValue(Source);
			}
			set {
				Source.GetType().GetProperty(name)?.SetValue(Source, value);
			}
		}
	}
}
