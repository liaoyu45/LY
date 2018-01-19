using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace LivingDB {
	public enum ModelState {
		NotBuilt = 0, WillAdd = 1, WillCreate = 2, Overload = 3
	}
	public abstract class DynamicModel {
		[NotMapped]
		protected internal abstract string TableName { get; }
		[NotMapped]
		protected internal virtual string SelfTableName => GetType().GetCustomAttribute<TableAttribute>(true)?.Name ?? GetType().Name + 's';
		[NotMapped]
		protected internal virtual int TableSize => 0x10;//000;
	}
}
