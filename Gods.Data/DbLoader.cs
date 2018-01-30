using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace LivingDB {
	public class DbLoader {
		public static List<IDbLoader> Loaders = new List<IDbLoader> { new DefaultDbLoader() };

		private static List<TypeCache> caches = new List<TypeCache>();
		internal static TypeCache GetCache(ShardDb db) {
			var type = db.GetType();
			if (caches.Any(e => e.cacheNamespace == type.Namespace)) {
				return null;
			}
			var f = caches.FirstOrDefault(e => e.BaseDbContextName == type.FullName);
			if (f != null) {
				return f;
			}
			var pn = ConfigurationManager.ConnectionStrings.OfType<ConnectionStringSettings>().FirstOrDefault(e => e.ConnectionString == db.Database.Connection.ConnectionString).ProviderName;
			var loader = Loaders.FirstOrDefault(i => i.ProviderName == pn);
			db.Database.ExecuteSqlCommand(loader.Prepare);
			var types = (from p in type.GetProperties()
						 let pt = p.PropertyType
						 where pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(DbSet<>)
						 select pt.GenericTypeArguments[0]).ToList();
			var tableToType = types.Where(t => t.BaseType.BaseType == typeof(DynamicModel)).ToDictionary(e => loader.GetTableName(e));//table to type
			Func<MainTableData[]> all = () => loader.GetRecord(tableToType.Keys.ToArray())
						.SelectMany(sql => db.Database.SqlQuery<RecordData>(sql))
						.GroupBy(e => e.Main)
						.ToDictionary(e => tableToType[e.Key].BaseType, e => e)
						.Select(e => new MainTableData(
							e.Key,
							loader.Size(e.Key),
							e.Value.Key,
							loader.CreateTable(e.Key),
							e.Value.Select(ee => new DetailTable(ee.Detail) {
								Count = ee.Count
							}).ToArray()
						)).ToArray();
			caches.Add(new TypeCache(type, loader, all));
			return caches.Last();
		}
	}
}
