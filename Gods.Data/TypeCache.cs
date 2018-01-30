using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LivingDB {
	class TypeCache {
		internal TypeCache(Type type, IDbLoader loader, Func<MainTableData[]> getTables) {
			BaseDbContextName = type.FullName;
			while (type.BaseType != null) {
				ll.Add(type.Assembly.Location);
				type = type.BaseType;
			}
			this.loader = loader;
			LoadTables = getTables;
		}

		public Func<MainTableData[]> LoadTables { get; }
		private List<string> ll = new List<string>(ls);
		private static readonly string[] ls = new[] {
			typeof(Enumerable).Assembly.Location,
			typeof(CSharpCodeProvider).Assembly.Location,
			typeof(TableAttribute).Assembly.Location,
			typeof(DbContext).Assembly.Location,
			typeof(ShardDb).Assembly.Location
		};

		public Task SaveChanges(IEnumerable<object> all) {
			if (!all.Any()) {
				return null;
			}
			var data = all.ToDictionary(e => e, loader.GetDynamicTableName);
			var sql = tables.Join(data, e => e.Type, e => e.Key.GetType().BaseType, (e, ee) => {
				return e.Add(ee.Value) ? e.Sql.Replace(e.TableName, ee.Value) : null;
			}).Distinct().Aggregate(string.Empty, (s, ss) => s + ss).Trim();
			Compile();
			var d = Activator.CreateInstance(dbtype) as ShardDb;
			if (sql.Any()) {
				return d.Database.ExecuteSqlCommandAsync(sql).ContinueWith(i => Save(d, data));
			} else {
				return Save(d, data);
			}
		}

		private Task Save(ShardDb d, Dictionary<object, string> data) {
			foreach (var item in data) {
				var type = dbsets.FirstOrDefault(e => e.Name == item.Value);
				if (type != null) {
					var newModel = Activator.CreateInstance(type);
					Gods.Him.CopyTo(item.Key, newModel);
					d.Set(type).Add(newModel);
				} else {
					d.Set(item.Key.GetType()).Add(item.Key);
				}
			}
			return d.SaveChangesAsync().ContinueWith(i => {
				ts = null;
				d.Dispose();
			});
		}

		private void Compile() {
			para.ReferencedAssemblies.Clear();
			para.ReferencedAssemblies.AddRange(ll.ToArray());
			para.ReferencedAssemblies.AddRange(tables.Select(e => e.Type).SelectMany(a => Gods.Him.GetBases(a, typeof(object)).Select(e => e.Assembly.Location)).ToArray());
			var classes = tables.SelectMany(t =>
					  t.Tables.Where(e => !e.IsMain).Select(d => $@"
namespace {cacheNamespace} {{
	[System.ComponentModel.DataAnnotations.Schema.Table(""{d.Name}"")]
	public class {d.Name} : {t.FullName} {{
	}}
}}"));
			var context = $@"
namespace {cacheNamespace} {{
	public class {nameof(Compile) + Math.Abs(Guid.NewGuid().GetHashCode())} : {BaseDbContextName} {{{tables.Aggregate(string.Empty, (s, t) => s + t.Tables.Where(e => !e.IsMain).Aggregate(string.Empty, (ss, d) => ss + $@"
		public System.Data.Entity.DbSet<{cacheNamespace + '.' + d.Name}> {d.Name} {{ get; set; }}"))}
	}}
}}";
			var ts = pr.CompileAssemblyFromSource(para, classes.Concat(new[] { context }).ToArray()).CompiledAssembly.ExportedTypes;
			dbtype = ts.First(e => e.BaseType.FullName == BaseDbContextName);
			dbsets = ts.Where(e => e != dbtype).ToList();
		}

		internal readonly string cacheNamespace = nameof(TypeCache) + DateTime.Now.Ticks;
		private CompilerParameters para = new CompilerParameters { GenerateInMemory = true };
		private CSharpCodeProvider pr = new CSharpCodeProvider();
		private MainTableData[] tables => ts ?? (ts = LoadTables());
		private IDbLoader loader;
		private IEnumerable<Type> dbsets;
		private Type dbtype;
		private MainTableData[] ts;

		public string BaseDbContextName { get; }

		public ModelState CheckState(object model) {
			var t = tables.FirstOrDefault(e => e.Type == model.GetType().BaseType);
			if (t == null) {
				return ModelState.NotBuilt;
			}
			var dynamicName = loader.GetDynamicTableName(model);
			var dt = t.Tables.FirstOrDefault(e => e.Name == dynamicName);
			if (dt == null) {
				return ModelState.AddNew;
			}
			if (dt.Count >= t.Max) {
				return ModelState.Overload;
			}
			return ModelState.AddModel;
		}
	}
}
