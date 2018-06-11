using System;

namespace Gods.Data {
	/// <summary>
	/// 使用默认多表存储时，定义建表语句和数据行上限。
	/// </summary>
	public class SqlAttribute : Attribute {
		/// <summary>
		/// 仅用于测试。
		/// </summary>
		/// <param name="sql">建表语句。</param>
		public SqlAttribute(string sql) : this(sql, 0x10) {
			Sql = sql;
		}
		/// <summary>
		/// Oo=-!'_'!-=oO
		/// </summary>
		/// <param name="sql">建表语句。</param>
		/// <param name="size">数据行上限。</param>
		public SqlAttribute(string sql, int size) {
			Sql = sql;
			Size = size;
		}
		/// <summary>
		/// 建表时执行的 sql
		/// </summary>
		public string Sql { get; }
		/// <summary>
		/// 单表存储数据行上限。
		/// </summary>
		public int Size { get; }
	}
}
