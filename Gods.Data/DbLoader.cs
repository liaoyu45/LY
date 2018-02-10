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
			Func<MainTableData[]> all1 = () => loader.GetRecord(tableToType.Keys.ToArray())
					.SelectMany(sql => db.Database.SqlQuery<DetailTable>(sql))
					.GroupBy(t => t.Main).ToDictionary(e => e, e => tableToType[e.Key].BaseType).Select(t => new MainTableData(
							   t.Value,
							   loader.Size(t.Value),
							   t.Key.Key,
							   loader.CreateTable(t.Value),
							   t.Key.ToArray()
							   )).ToArray();
			caches.Add(new TypeCache(type, loader, all1));
			return caches.Last();
		}
	}
}
