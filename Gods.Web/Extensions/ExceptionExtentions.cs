using System;

namespace Javascript.Extensions {
	public static class ExceptionExtentions {
		public static Exception Final(this Exception e) {
			return e.InnerException == null ? e : Final(e.InnerException);
		}
	}
}
