using Me.Inside;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Me.Real {
	public class Universe : DbContext {
		public DbSet<Effort> Efforts { get; set; }
		public DbSet<Plan> Plans { get; set; }
		public DbSet<Tag> Tags { get; set; }

		static Universe() {
			Database.SetInitializer(new TheBigBang());
		}

		public Universe() : base(nameof(Me)) { }

		public static T AddOrUpdate<T>(Dictionary<string, object> source, params object[] ids) where T : class, new() {
			using (var u = new Universe()) {
				var update = ids?.Length > 0;
				var r = update ? u.Set<T>().Find(ids) : new T();
				foreach (var item in source) {
					typeof(T).GetProperty(item.Key).SetValue(r, item.Value);
				}
				if (!update) {
					u.Set<T>().Add(r);
				}
				u.SaveChanges();
				return r;
			}
		}

		public static T Using<T>(Func<Universe, T> func) {
			using (var d = new Universe()) {
				var r = func(d);
				if (d.ChangeTracker.HasChanges()) {
					d.SaveChanges();
				}
				return r;
			}
		}

		public static void Using(Action<Universe> action) {
			Using(d => {
				action(d);
				var c = d.ChangeTracker.Entries().Count();
				if (c > 0) {
					d.SaveChanges();
				}
				return c;
			});
		}
	}
}
