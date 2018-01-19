using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace LivingDB {
	public abstract partial class ShardDb {
		public virtual void WillNotMigrate() {
			Database.ExecuteSqlCommand(@"
if 0 < (select count(*) from sysobjects where xtype='U' and name='__MigrationHistory')
	drop table __MigrationHistory");
		}


		protected internal virtual string QuerySimiliarTables(Type m) {
			var name = m.GetCustomAttribute<TableAttribute>(true)?.Name ?? m.Name + 's';
			return $"select name from sysobjects where xtype='U' and substring(name,1,{name.Length})='{name}' and len(name)>{name.Length}";
		}
	}
}
