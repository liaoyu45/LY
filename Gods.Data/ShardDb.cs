using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace LivingDB {
	public abstract partial class ShardDb : DbContext {
		private List<object> pending = new List<object>();
		private TypeCache thisCache;

		public ShardDb(string dbName) : base(dbName) {
			thisCache = DbLoader.GetCache(this);
		}

		public override Task<int> SaveChangesAsync() {
			thisCache?.SaveChanges(pending);
			return base.SaveChangesAsync();
		}
		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
			thisCache?.SaveChanges(pending);
			return base.SaveChangesAsync(cancellationToken);
		}

		public override int SaveChanges() {
			thisCache?.SaveChanges(pending);
			return base.SaveChanges();
		}

		public IEnumerable<object> Pending => pending;

		public ModelState AddNew(DynamicModel model, bool limit) {
			var r = thisCache?.CheckState(model) ?? ModelState.Unavailable;
			if (r < 0) {
				return r;
			}
			if (r == ModelState.Overload) {
				if (limit) {
					return r;
				}
			}
			if (r == ModelState.AddNew) {
				var arg = new DynamicModelEventArgs { Model = model };
				OnTableCreated?.Invoke(this, arg);
				if (arg.Canceled) {
					return ModelState.Canceled;
				}
			}
			pending.Add(model);
			return r;
		}

		public event EventHandler<DynamicModelEventArgs> OnTableCreated;
	}
}
