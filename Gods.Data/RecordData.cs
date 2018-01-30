﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LivingDB {
	class RecordData {
		public string Main { get; set; }
		public string Detail { get; set; }
		public int Count { get; set; }
	}
	class MainTableData {
		public MainTableData(Type type, int max, string tableName, string sql, params DetailTable[] tables) {
			TableName = tableName;
			Type = type;
			Sql = sql;
			Max = max;
			tables.First(e => e.Name == tableName).IsMain = true;
			Tables = tables.ToList();
		}
		public Type Type { get; }
		public int Max { get; }
		public string FullName => Type.FullName;
		public string TableName { get; }
		public IEnumerable<DetailTable> Tables { get; }
		public string Sql { get; }

		public bool Add(string table) {
			if (Tables.Any(e => e.Name == table)) {
				return false;
			}
			((List<DetailTable>)Tables).Add(new DetailTable(table));
			return true;
		}
	}
	class DetailTable {
		public DetailTable(string name) {
			Name = name;
		}
		public string Name { get; }
		public bool IsMain { get; set; }
		public int Count { get; set; }
	}
}
