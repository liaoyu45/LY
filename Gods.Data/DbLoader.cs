using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace LivingDB {
	public class DbLoader {
		public static List<IDbLoader> Loaders = new List<IDbLoader> { new DefaultDbLoader() };

		private static Dictionary<string, IDbLoader> loaders = new Dictionary<string, IDbLoader>();

		static DbLoader() {
			Loaders.Add(new DefaultDbLoader());
		}

		public static IDbLoader GetLoader(string code) {
			return loaders[code];
		}

		private static List<TypeCache> caches = new List<TypeCache>();
		internal static TypeCache GetCache(ShardDb db) {
			var type = db.GetType();
			var f = caches.FirstOrDefault(e => e.BaseDbContextName == type.FullName);
			if (f != null) {
				return f;
			}
			var pn = ConfigurationManager.ConnectionStrings.OfType<ConnectionStringSettings>().FirstOrDefault(e => e.ConnectionString == db.Database.Connection.ConnectionString).ProviderName;
			var loader = loaders[type.FullName] = Loaders.FirstOrDefault(i => i.ProviderName == pn);
			db.Database.ExecuteSqlCommand(loader.Prepare);
			var types = (from p in type.GetProperties()
						 let pt = p.PropertyType
						 where pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(DbSet<>)
						 select pt.GenericTypeArguments[0]).ToList();
			var tables = types.Where(t => t.BaseType.BaseType == typeof(DynamicModel)).ToList();
			tables.ToDictionary(e => e, e => loader.GetTableName(e));
			var all = loader.GetRecord(tables.Select(loader.GetTableName).ToArray())
						.SelectMany(sql => db.Database.SqlQuery<RecordData>(sql))
						.GroupBy(e => e.Main)
						.ToDictionary(e => tables.First(aaa => loader.GetTableName(aaa) == e.Key).BaseType, e => e)
						.Select(e => new MainTableData(tables.First(aaa => loader.GetTableName(aaa) == e.Value.Key).BaseType, loader.Size(e.Key)) {
							TableName = e.Value.Key,
							Tables = new List<DetailTable>(e.Value.Select(ee =>
								new DetailTable {
									IsMain = e.Value.Key == ee.Detail,
									Name = ee.Detail,
									Count = ee.Count
								}))
						});
			types.Add(type);
			caches.Add(new TypeCache(type.FullName, all, types.ToArray()));
			return caches.Last();
		}
	}
}
