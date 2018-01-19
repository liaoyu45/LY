using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LivingDB {
	class TypeCache {
		public TypeCache(Type t, Dictionary<Type, List<string>> all) {
			OrigDbType = t;
			Tables = all;
			para.ReferencedAssemblies.Add("System.Core.dll");
			para.ReferencedAssemblies.Add("System.dll");
			all.Keys.Concat(Gods.Him.GetBases(t, typeof(object)))
				.Select(e => e.Assembly.Location).Distinct().ToList()
				.ForEach(item => para.ReferencedAssemblies.Add(item));
			Compile();
		}

		public void Compile() {
			cacheDbType = null;
			var dbsets = Tables.Aggregate(string.Empty, (s, a) =>
				s + a.Value.Aggregate(string.Empty, (ss, aa) =>
					ss + $@"public System.Data.Entity.DbSet<{aa}> {aa} {{ get; set; }}"));
			var codes = Tables.SelectMany(s =>
				   s.Value.Select(aa => $@"
public class {aa} : {s.Key.BaseType.FullName} {{}}"))
				.Concat(new[] { $@"
public class {cachDbTypeName} : {OrigDbType.FullName} {{
	{dbsets}
}}" }).ToArray();
			var cr = pr.CompileAssemblyFromSource(para, codes);
			cacheAssembly = cr.CompiledAssembly;
		}

		private CompilerParameters para = new CompilerParameters { GenerateInMemory = true };
		private CSharpCodeProvider pr = new CSharpCodeProvider();
		private Assembly cacheAssembly;
		private string cachDbTypeName => OrigDbType.Name + Tables.Count;
		private Type cacheDbType;
		private Dictionary<string, int> sizeRecords;

		public Type OrigDbType { get; }
		public Type CacheDbType => cacheDbType ?? (cacheDbType = cacheAssembly.GetType(cachDbTypeName));
		public Dictionary<string, int> SizeRecords => sizeRecords ?? (sizeRecords = Tables.SelectMany(p => p.Value).ToDictionary(e => e, e => 0));
		public Dictionary<Type, List<string>> Tables { get; }

		public DynamicModel MapModel(DynamicModel m) {
			var mm = Activator.CreateInstance(cacheAssembly.GetType(m.TableName));
			Gods.Him.CopyTo(m, mm);
			return mm as DynamicModel;
		}

		public void AddType(Type t, string n) {
			var c = $@"public class {n} : {t.BaseType.FullName}{{}}";
		}
		/// <summary>
		/// 检查要进行真实读写的数据对象。
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public ModelState CheckState(DynamicModel m) {
			if (Tables.ContainsKey(m.GetType())) {
				if (SizeRecords.ContainsKey(m.TableName)) {
					if (SizeRecords[m.TableName] < m.TableSize) {
						return ModelState.WillAdd;
					}
					return ModelState.Overload;
				}
				return ModelState.WillCreate;
			}
			return ModelState.NotBuilt;
		}
	}
}
