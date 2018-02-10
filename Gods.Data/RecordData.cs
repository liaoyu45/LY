using System;
using System.Collections.Generic;
using System.Linq;

namespace LivingDB {
	class MainTableData {
		public MainTableData(Type type, int max, string tableName, string sql, params DetailTable[] tables) {
			TableName = tableName;
			Type = type;
			Sql = sql;
			Max = max;
			Tables = tables.ToList();
		}
		public Type Type { get; }
		public int Max { get; }
		public string FullName => Type.FullName;
		public string TableName { get; }
		public IEnumerable<DetailTable> Tables { get; set; }
		public string Sql { get; }

		public bool Add(string table) {
			if (Tables.Any(e => e.Detail == table)) {
				return false;
			}
			((List<DetailTable>)Tables).Add(new DetailTable(table));
			return true;
		}
	}
	class DetailTable {
		public DetailTable() { }
		public DetailTable(string name) {
			Detail = name;
		}
		public string Main { get; set; }
		public string Detail { get; set; }
		public int Count { get; set; }

		public bool IsMain => Main == Detail;
	}
}
