using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;

namespace LivingDB {
	public abstract partial class ShardDb : DbContext {
		private TypeCache thisCache;
		private IDbLoader dbLoader;

		public ShardDb(string dbName) : base(dbName) {
			if (!TypeCache.IsCache(GetType())) {
				thisCache = DbLoader.GetCache(this);
				dbLoader = DbLoader.GetLoader(GetType().FullName);
			}
		}

		public override int SaveChanges() {
			thisCache?.SaveChanges(pending);
			thisCache?.SaveChanges();
			return base.SaveChanges();
		}

		public T Add<T>(T m) where T : DynamicModel {
			return Add(m, true);
		}

		public T Add<T>(T model, bool limit) where T : DynamicModel {
			return typeof(ShardDb).GetMethod(CheckState(model, limit).ToString(), (BindingFlags)36)?.Invoke(this, new object[] { model }) as T;
		}

		public ModelState CheckState(DynamicModel model, bool limit) {
			var type = model.GetType();
			var newTable = dbLoader.GetDynamicTableName(model);
			return thisCache?.CheckState(dbLoader.GetTableName(type), newTable, limit) ?? ModelState.Unavailable;
		}

		private List<PendingData> pending = new List<PendingData>();
		public ModelState AddNew(DynamicModel model, bool limit) {
			var type = model.GetType();
			var newTable = dbLoader.GetDynamicTableName(model);
			var r = thisCache?.CheckState(dbLoader.GetTableName(type), newTable, limit) ?? ModelState.Unavailable;
			if (r < 0) {
				pending.Add(new PendingData {
					IsNew = r == ModelState.RecreateInnerDb,
					Model = model,
					TableName = newTable
				});
			}
			return r;
		}

		public event EventHandler<DynamicModelEventArgs> OnTableCreated;

		private void SaveMain(object model) {
			Set(model.GetType()).Add(model);
		}

		private void RecreateInnerDb(object model) {
			var newTable = dbLoader.GetDynamicTableName(model);
			var args = new DynamicModelEventArgs {
				TableName = newTable,
				Model = model
			};
			OnTableCreated?.Invoke(this, args);
			if (args.Canceled) {
				return;
			}
			var ttt = dbLoader.GetTableName(model.GetType());
			Database.ExecuteSqlCommand(dbLoader.CreateTable(model.GetType()).Replace(ttt, newTable));
			thisCache.RecreateInnerDb(ttt, newTable);
			AddModel(model);
		}

		private void AddModel(object model) {
			thisCache.AddModel(model, dbLoader.GetDynamicTableName(model));
		}
	}
}
