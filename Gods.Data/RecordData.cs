using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Data {
	class MainTableData {
		public MainTableData(Type type, int max, string tableName, string sql, params DetailTableData[] tables) {
			TableName = tableName;
			Type = type;
			Sql = sql;
			Max = max;
			Tables = tables.ToList();
		}
		/// <summary>
		/// 代码中的数据类型，是 <see cref="System.Data.Entity.DbSet{TEntity}"/> 的泛型类型参数。
		/// </summary>
		public Type Type { get; }
		/// <summary>
		/// 每个 <see cref="DetailTableData"/> 中的数据量上限。
		/// </summary>
		public int Max { get; }
		/// <summary>
		/// <see cref="Type"/> 的 <see cref="Type.FullName"/>。
		/// </summary>
		public string FullName => Type.FullName;
		/// <summary>
		/// 与 <see cref="Type"/> 相对应。
		/// </summary>
		public string TableName { get; }
		/// <summary>
		/// 包含了所有数据表信息。
		/// </summary>
		public IEnumerable<DetailTableData> Tables { get; set; }
		/// <summary>
		/// 用于生成新表时，将替换 <see cref="TableName"/>。
		/// </summary>
		public string Sql { get; }

		public bool Add(string table) {
			if (Tables.Any(e => e.Detail == table)) {
				return false;
			}
			(Tables as List<DetailTableData>)?.Add(new DetailTableData(table));
			return true;
		}
	}
	/// <summary>
	/// 每个详细数据表的信息。
	/// </summary>
	class DetailTableData {
		public DetailTableData() { }
		public DetailTableData(string name) {
			Detail = name;
		}
		/// <summary>
		/// 主表名称。
		/// </summary>
		public string Main { get; set; }
		/// <summary>
		/// 分表名称
		/// </summary>
		public string Detail { get; set; }
		/// <summary>
		/// 当前分表的数据量。
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// 如果此分表为主表，返回true。
		/// </summary>
		public bool IsMain => Main == Detail;
	}
}
