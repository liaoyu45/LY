using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace LivingDB {
	class TypeCache {
		private static readonly List<string> cacheNamespaces = new List<string>();
		public static bool IsCache(Type type) =>
			cacheNamespaces.Contains(type.Namespace);

		~TypeCache() {
			db?.Dispose();
		}

		internal TypeCache(string dbTypeFullName, IEnumerable<MainTableData> tables, params Type[] ass) {
			cacheNamespaces.Add(cacheNamespace);
			BaseDbContextName = dbTypeFullName;
			cachDbTypeName = dbTypeFullName.Split('.').Last() + tables.Count();
			this.tables = tables.ToArray();
			para.ReferencedAssemblies.Add(typeof(Enumerable).Assembly.Location);
			para.ReferencedAssemblies.Add(typeof(CSharpCodeProvider).Assembly.Location);
			para.ReferencedAssemblies.Add(typeof(TableAttribute).Assembly.Location);
			ass.SelectMany(a => Gods.Him.GetBases(a, typeof(object)).Select(e => e.Assembly.Location))
				.ToList().ForEach(e =>
					para.ReferencedAssemblies.Add(e));
			Compile();
		}

		public void SaveChanges(IEnumerable<PendingData> all) {
			all.Where(e => e.IsNew);
		}

		public void SaveChanges() {
			if (db?.ChangeTracker.HasChanges() == true) {
				db.SaveChanges();
				db.Dispose();
				db = null;
			}
		}

		private void Compile() {
			var classes = tables.SelectMany(s =>
					  s.Tables.Where(e => !e.IsMain).Select(tablename => $@"
namespace {cacheNamespace} {{
	[System.ComponentModel.DataAnnotations.Schema.Table(""{tablename.Name}"")]
	public class {tablename.Name} : {s.FullName} {{
	}}
}}"));
			var context = $@"
namespace {cacheNamespace} {{
	public class {cachDbTypeName} : {BaseDbContextName} {{{tables.Aggregate(string.Empty, (s, a) => s + a.Tables.Where(e => !e.IsMain).Aggregate(string.Empty, (ss, aa) => ss + $@"
		public System.Data.Entity.DbSet<{cacheNamespace + '.' + aa.Name}> {aa.Name} {{ get; set; }}"))}
	}}
}}";
			var r = pr.CompileAssemblyFromSource(para, classes.Concat(new[] { context }).ToArray());
			cacheAssembly = r.CompiledAssembly;
		}

		public void RecreateInnerDb(string tableName, string name) {
			tables.First(e => e.TableName == tableName).Tables.Add(new DetailTable { Name = name });
			SaveChanges();
			Compile();
		}

		private string cacheNamespace = nameof(TypeCache) + DateTime.Now.Ticks;
		private CompilerParameters para = new CompilerParameters { GenerateInMemory = true };
		private CSharpCodeProvider pr = new CSharpCodeProvider();
		private Assembly cacheAssembly;
		private string cachDbTypeName;
		public string BaseDbContextName { get; }
		private MainTableData[] tables;

		private ShardDb db;
		private ShardDb NewDB => db ?? (db =
			Activator.CreateInstance(cacheAssembly.GetType(cacheNamespace + '.' + cachDbTypeName)) as ShardDb);

		public object AddModel(object model, string tableName) {
			var type = cacheAssembly.GetType(cacheNamespace + '.' + tableName);
			var newModel = Activator.CreateInstance(type);
			Gods.Him.CopyTo(model, newModel);
			return NewDB.Set(type).Add(newModel);
		}
		/// <summary>
		/// 检查即将添加的数据的状态。
		/// </summary>
		/// <param name="tableName">数据所属的结构表。</param>
		/// <param name="dynamicName">要填充到的表名。</param>
		/// <param name="limit">是否限制行数上限。参见 <see cref="IDbLoader.Size(Type)"/>。</param>
		/// <returns>要添加的数据的状态。不包含 <see cref="ModelState.Unavailable"/>。</returns>
		public ModelState CheckState(string tableName, string dynamicName, bool limit) {
			var t = tables.FirstOrDefault(e => e.TableName == tableName);
			if (t == null) {
				return ModelState.NotBuilt;
			}
			var dt = t.Tables.FirstOrDefault(e => e.Name == dynamicName);
			if (dt == null) {
				return ModelState.RecreateInnerDb;
			}
			if (dt.IsMain) {
				return ModelState.SaveMain;
			}
			if (!limit || dt.Count < t.Max) {
				return ModelState.AddModel;
			}
			return ModelState.Overload;
		}
	}
}
