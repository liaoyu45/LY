using System;

namespace Gods.Data {
	/// <summary>
	/// 定义了多表存储的规则。
	/// </summary>
	public interface IDbLoader {
		/// <summary>
		/// 对应连接字符串中的 providerName。
		/// </summary>
		string ProviderName { get; }
		/// <summary>
		/// 数据库初始化时执行的 sql。
		/// </summary>
		string Prepare { get; }
		/// <summary>
		/// 返回获取全部可扩展表中已存的数据行数的 sql，返回的数据集格式应为：([Main] nvarchar,[Detail] nvarchar,[Count] int)。
		/// </summary>
		/// <param name="tables">全部基础结构表的名称。</param>
		/// <returns>可执行的 sql 的数组。</returns>
		string[] GetRecord(params string[] tables);
		/// <summary>
		/// 将代码中定义的类型映射到数据库中定义了基础结构的表。
		/// </summary>
		/// <param name="type"><see cref="ShardDb"/> 的子类中 <see cref="System.Data.Entity.DbSet{TEntity}"/> 的泛型参数类型。</param>
		/// <returns>具有基础结构的表的名称。</returns>
		string GetTableName(Type type);
		/// <summary>
		/// 用于定义生成新名的规则。
		/// </summary>
		/// <param name="model">等待填充的数据。</param>
		/// <returns>一个表名，数据将填充到此表。</returns>
		string GetDynamicTableName(object model);
		/// <summary>
		/// 基础结构表的创建 sql。当需求新表存储数据时将复制其表结构。
		/// </summary>
		/// <param name="type">要存储到新表中去的数据的类型。</param>
		/// <returns>基础结构表的创建 sql，其中出现过的所有原表名将使用新表名替换，如在包含了原表名的外键名。</returns>
		string CreateTable(Type type);
		/// <summary>
		/// 返回基础结构表最大数据行数。
		/// </summary>
		/// <param name="type">代码中定义的类型，和基础结构表一一相应。</param>
		/// <returns>基础结构表最大数据行数。</returns>
		int Size(Type type);
		string CreateTable(string sqlFormat, object model);
	}
}