using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LivingDB {
	public abstract partial class ShardDb : DbContext {
		private static List<TypeCache> allCache = new List<TypeCache>();

		private TypeCache ThisCache => allCache.FirstOrDefault(e => e.OrigDbType == GetType());
		private ShardDb innerdb;
		public ShardDb InnerDB => ThisCache == null ? null : innerdb ?? (innerdb = Activator.CreateInstance(ThisCache.CacheDbType) as ShardDb);

		public ShardDb(string nameOrConnectionString) : base(nameOrConnectionString) {
			if (ThisCache != null) {
				return;
			}
			var dbtype = GetType();
			if (dbtype.BaseType != typeof(ShardDb)) {
				return;
			}
			var ts = from p in dbtype.GetProperties()
					 let pt = p.PropertyType
					 where pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(DbSet<>)
					 let at = pt.GenericTypeArguments[0]
					 where at.BaseType.BaseType == typeof(DynamicModel)
					 select at;
			var dictionary = ts.ToDictionary(t => t, t => Database.SqlQuery<string>(QuerySimiliarTables(t)).ToList());
			allCache.Add(new TypeCache(dbtype, dictionary));
		}

		public override int SaveChanges() {
			InnerDB?.SaveChanges();
			return base.SaveChanges();
		}

		private void RecreateInnerDb(DynamicModel m) {
			Database.ExecuteSqlCommand($"select * into {m.TableName} from {m.SelfTableName}");
			ThisCache.Tables[m.GetType()].Add(m.TableName);
			ThisCache.Compile();
			innerdb = null;
			AddModel(m);
		}
		private DynamicModel AddModel(DynamicModel m) {
			m = ThisCache.MapModel(m);
			return InnerDB.Set(m.GetType()).Add(m) as DynamicModel;
		}
		public bool Add(DynamicModel m) {
			if (GetType().BaseType != typeof(ShardDb)) {
				throw null;
			}
			switch (ThisCache.CheckState(m)) {
				case ModelState.Overload:
					return false;
				default:
				case ModelState.NotBuilt:
					return false;
				case ModelState.WillAdd:
					AddModel(m);
					return true;
				case ModelState.WillCreate:
					RecreateInnerDb(m);
					return true;
			}
		}
	}
}
