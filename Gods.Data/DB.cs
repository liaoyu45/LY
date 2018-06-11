using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Data {
	class DB {
		private ShardDb db;

		public ShardDb D => db ?? (db = Activator.CreateInstance(DbType) as ShardDb);
		public IEnumerable<Type> DbSets { get; set; }
		public Type DbType { get; set; }
		public void Exec(Action<ShardDb> db) {
			var d = Activator.CreateInstance(DbType) as ShardDb;
			db(d);
			d.Dispose();
		}

		public void Save(Dictionary<object, string> data) {
			Exec(d => {
				var aaa = new Dictionary<object, object>();
				foreach (var item in data) {
					var type = DbSets.FirstOrDefault(e => e.Name == item.Value);
					if (type != null) {
						var newModel = Activator.CreateInstance(type);
						Gods.Him.CopyTo(item.Key, newModel);
						d.Set(type).Add(newModel);
						aaa[item.Key] = newModel;
					} else {
						d.Set(item.Key.GetType()).Add(item.Key);
					}
				}
				d.SaveChanges();
				foreach (var item in aaa) {
					Gods.Him.CopyTo(item.Value, item.Key);
				}
			});
		}
	}
}
