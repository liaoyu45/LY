namespace Gods.Data {
	/// <summary>
	/// 要添加的数据的状态。
	/// </summary>
	public enum ModelState {
		/// <summary>
		/// 即将添加新数据。
		/// </summary>
		AddModel = 1,
		/// <summary>
		/// 即将生成新的运行时数据库上下文实例。
		/// </summary>
		AddNew = 2,
		/// <summary>
		/// 单表数据行数量超出上限。
		/// </summary>
		Overload = 3,
		/// <summary>
		/// 原数据库上下文中不存在对应的类型。
		/// </summary>
		NotBuilt = -1,
		/// <summary>
		/// 动态数据库上下文实例不应直接使用。
		/// </summary>
		Unavailable = -2,
		/// <summary>
		/// 事件触发取消。
		/// </summary>
		Canceled = -3
	}
	public abstract class DynamicModel { }
}
