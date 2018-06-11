using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Gods.Data {
	public abstract class HubContext<DB, T>
		where DB : DbContext, new()
		where T : class {
		public static void Remove(object idOrObj) {
			using (var db = new DB()) {
				if (idOrObj.GetType() == typeof(T)) {
					db.Set<T>().Remove(idOrObj as T);
					db.SaveChanges();
				} else {
					var t = db.Set<T>().Find(idOrObj);
					if (t != null) {
						db.Set<T>().Remove(t);
					}
					db.SaveChanges();
				}
			}
		}

		public static T AddOrChange(object source, params object[] ids) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}
			using (var db = new DB()) {
				var a = db.Set<T>().Find(ids) ?? Activator.CreateInstance<T>();
				Him.CopyTo(source, a);
				if (ids == null || !ids.Any()) {
					db.Set<T>().Add(a);
				}
				db.SaveChanges();
				return a;
			}
		}

		public static void Execute(Action<T> func, params object[] ids) {
			using (var db = new DB()) {
				var t = db.Set<T>().Find(ids);
				if (t == null) {
					return;
				}
				var props = typeof(T).GetProperties();
				var oldValues = props.Select(p => p.GetValue(t)).ToList();
				func(t);
				var newValues = props.Select(p => p.GetValue(t, null) ?? string.Empty).ToList();
				for (var i = 0; i < oldValues.Count; i++) {
					if (newValues[i]?.GetHashCode() != oldValues[i]?.GetHashCode()) {
						db.SaveChanges();
						break;
					}
				}
			}
		}

		public static T First() {
			using (var db = new DB()) {
				return db.Set<T>().FirstOrDefault();
			}
		}

		public static T First(params object[] ids) {
			using (var db = new DB()) {
				return db.Set<T>().Find(ids);
			}
		}

		public static T First(Func<T, bool> predicate) {
			using (var db = new DB()) {
				return db.Set<T>().FirstOrDefault(predicate);
			}
		}

		public static T First<P>(Func<T, bool> predicate, Func<T, P> orderby) =>
			First(predicate, orderby, false);

		public static T First<P>(Func<T, bool> predicate, Func<T, P> orderby, bool desc) {
			using (var db = new DB()) {
				T r;
				if (desc) {
					r = db.Set<T>().Where(predicate).OrderByDescending(orderby).FirstOrDefault();
				} else {
					r = db.Set<T>().Where(predicate).OrderBy(orderby).FirstOrDefault();
				}
				return r;
			}
		}

		public static int Count() {
			using (var db = new DB()) {
				return db.Set<T>().Count();
			}
		}

		public static int Count(Func<T, bool> prediacate) {
			using (var db = new DB()) {
				return db.Set<T>().Count(prediacate);
			}
		}

		public static IEnumerable<T> All() {
			using (var db = new DB()) {
				var r = db.Set<T>().ToList();
				return r;
			}
		}

		public static bool Exists(object id) {
			using (var db = new DB()) {
				var r = db.Set<T>().Find(id);
				return r != null;
			}
		}

		public static bool Exists(Func<T, bool> where) {
			using (var db = new DB()) {
				var r = db.Set<T>().Any(where);
				return r;
			}
		}
	}

	public abstract class HubContext<DB>
		where DB : DbContext, new() {
		public static void Remove<T>(object idOrObj) where T : class {
			using (var db = new DB()) {
				if (idOrObj.GetType() == typeof(T)) {
					db.Set<T>().Remove(idOrObj as T);
					db.SaveChanges();
				} else {
					var t = db.Set<T>().Find(idOrObj);
					if (t != null) {
						db.Set<T>().Remove(t);
					}
					db.SaveChanges();
				}
			}
		}
		public static T AddOrChange<T>(object source, params object[] ids) where T : class {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}
			using (var db = new DB()) {
				var a = db.Set<T>().Find(ids) ?? Activator.CreateInstance<T>();
				Him.CopyTo(source, a);
				if (ids == null || !ids.Any()) {
					db.Set<T>().Add(a);
				}
				db.SaveChanges();
				return a;
			}
		}

		public static void Execute<T>(Action<T> func, params object[] ids) where T : class {
			using (var db = new DB()) {
				var t = db.Set<T>().Find(ids);
				if (t == null) {
					return;
				}
				var props = typeof(T).GetProperties();
				var oldValues = props.Select(p => p.GetValue(t)).ToList();
				func(t);
				var newValues = props.Select(p => p.GetValue(t, null) ?? string.Empty).ToList();
				for (var i = 0; i < oldValues.Count; i++) {
					if (newValues[i]?.GetHashCode() != oldValues[i]?.GetHashCode()) {
						db.SaveChanges();
						break;
					}
				}
			}
		}

		public static T First<T>() where T : class {
			using (var db = new DB()) {
				return db.Set<T>().FirstOrDefault();
			}
		}

		public static T First<T>(params object[] ids) where T : class {
			using (var db = new DB()) {
				return db.Set<T>().Find(ids);
			}
		}

		public static T First<T>(Func<T, bool> predicate) where T : class {
			using (var db = new DB()) {
				return db.Set<T>().FirstOrDefault(predicate);
			}
		}

		public static T First<T, P>(Func<T, bool> predicate, Func<T, P> orderby) where T : class =>
			First(predicate, orderby, false);

		public static T First<T, P>(Func<T, bool> predicate, Func<T, P> orderby, bool desc) where T : class {
			using (var db = new DB()) {
				T r;
				if (desc) {
					r = db.Set<T>().Where(predicate).OrderByDescending(orderby).FirstOrDefault();
				} else {
					r = db.Set<T>().Where(predicate).OrderBy(orderby).FirstOrDefault();
				}
				return r;
			}
		}

		public static int Count<T>() where T : class {
			using (var db = new DB()) {
				return db.Set<T>().Count();
			}
		}

		public static int Count<T>(Func<T, bool> prediacate) where T : class {
			using (var db = new DB()) {
				return db.Set<T>().Count(prediacate);
			}
		}

		public static IEnumerable<T> All<T>() where T : class {
			using (var db = new DB()) {
				var r = db.Set<T>().ToList();
				return r;
			}
		}
		public static bool Exists<T>(object id) where T : class =>
			First<T>(id) != null;

		public static bool Exists<T>(Func<T, bool> where) where T : class =>
			First<T>(where) != null;
	}
}
