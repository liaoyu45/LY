using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace LivingDB {
	class TypeCache {
		internal TypeCache(Type dbType, IDbLoader loader, Func<MainTableData[]> getTables) {
			BaseDbContextName = dbType.FullName;
			while (dbType.BaseType != null) {
				assemblies.Add(dbType.Assembly.Location);
				dbType = dbType.BaseType;
			}
			this.loader = loader;
			loadTables = getTables;
		}

		public void SaveChanges(IEnumerable<object> pending) {
			if (!pending.Any()) {
				return;
			}
			var data = pending.ToDictionary(e => e, loader.GetDynamicTableName);
			var sql = tables.Join(data, e => e.Type, e => e.Key.GetType().BaseType, (e, ee) => {
				return e.Add(ee.Value) ? e.Sql.Replace(e.TableName, ee.Value) : null;
			}).Distinct().Aggregate(string.Empty, (s, ss) => s + ss).Trim();
			var db = DB;
			if (sql.Any()) {
				this.db = null;
				db.Exec(e => e.Database.ExecuteSqlCommand(sql));
			}
			db.Save(data);
		}


		private List<string> assemblies = new List<string>(baseAssemblies);
		private static readonly string[] baseAssemblies = new[] {
			typeof(Enumerable).Assembly.Location,
			typeof(CSharpCodeProvider).Assembly.Location,
			typeof(TableAttribute).Assembly.Location,
			typeof(DbContext).Assembly.Location,
			typeof(ShardDb).Assembly.Location
		};

		private Func<MainTableData[]> loadTables { get; }
		private IDbLoader loader;
		private MainTableData[] ts;
		private MainTableData[] tables => ts ?? (ts = loadTables());
		private DB db;
		private DB DB {
			get {
				if (db != null) {
					return db;
				}
				var para = new CompilerParameters { GenerateInMemory = true };
				para.ReferencedAssemblies.Clear();
				para.ReferencedAssemblies.AddRange(assemblies.ToArray());
				para.ReferencedAssemblies.AddRange(tables.Select(e => e.Type).SelectMany(a => Gods.Him.GetBases(a, typeof(object)).Select(e => e.Assembly.Location)).ToArray());
				var classes = tables.SelectMany(t =>
						  t.Tables.Where(e => !e.IsMain).Select(d => $@"
namespace {cacheNamespace} {{
	[System.ComponentModel.DataAnnotations.Schema.Table(""{d.Detail}"")]
	public class {d.Detail} : {t.FullName} {{
	}}
}}"));
				var context = $@"
namespace {cacheNamespace} {{
	public class {nameof(TypeCache) + Math.Abs(Guid.NewGuid().GetHashCode())} : {BaseDbContextName} {{{tables.Aggregate(string.Empty, (s, t) => s + t.Tables.Where(e => !e.IsMain).Aggregate(string.Empty, (ss, d) => ss + $@"
		public System.Data.Entity.DbSet<{d.Detail}> {d.Detail} {{ get; set; }}"))}
	}}
}}";
				var pr = new CSharpCodeProvider();
				var ts = pr.CompileAssemblyFromSource(para, classes.Concat(new[] { context }).ToArray()).CompiledAssembly.ExportedTypes;
				pr.Dispose();
				return db = new DB {
					DbSets = ts.Where(e => e.BaseType.FullName != BaseDbContextName),
					DbType = ts.First(e => e.BaseType.FullName == BaseDbContextName)
				};
			}
		}

		public readonly string cacheNamespace = nameof(TypeCache) + DateTime.Now.Ticks;
		public string BaseDbContextName { get; }

		public ModelState CheckState(object model) {
			var t = tables.FirstOrDefault(e => e.Type == model.GetType().BaseType);
			if (t == null) {
				return ModelState.NotBuilt;
			}
			var dynamicName = loader.GetDynamicTableName(model);
			var dt = t.Tables.FirstOrDefault(e => e.Detail == dynamicName);
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
