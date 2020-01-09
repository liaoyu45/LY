using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gods {
	public class SafeCaller {
		protected object target;
		private readonly Assembly s;

		public SafeCaller(object typeOrInstance, bool debug = false) {
			target = typeOrInstance;
			if (IsTemp()) {
				return;
			}
			var t = typeOrInstance is Type ? typeOrInstance as Type : typeOrInstance.GetType();
			using var compiler = new Microsoft.CSharp.CSharpCodeProvider();
			var @namespace = nameof(typeOrInstance) + DateTime.Now.Ticks;
			var file = @namespace + ".dll";
			var ops = new CompilerParameters() {
				OutputAssembly = file,
			};
			ops.ReferencedAssemblies.Add(GetType().Assembly.Location);
			const string ps = nameof(ps);
			var sources = t.GetMethods().Where(e => e.DeclaringType == t).Select(m => {
				var callerStr = m.IsStatic ? t.FullName : $"({nameof(GetTarget)}() as {t.FullName})";
				var eee = m.GetParameters();
				var args = eee.Length == 0 ? string.Empty : eee.Select((e, i) => $"({e.ParameterType.FullName}){ps}[{i}], ").Aggregate((s, ss) => s + ss).Trim(',', ' ');
				var className = MapHashCode(m.Name, m.GetParameters().Select(e => e.ParameterType));
				var @return = m.ReturnType == typeof(void) ? string.Empty : "return ";
				return $@"
namespace {@namespace} {{
	public class {className} : {typeof(SafeCaller).FullName} {{
		public {className} (object i) : base(i) {{ }}

		[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
		[System.Security.SecurityCritical]
		public override object Call(string method, params object[] {ps}) {{{(debug ? $@"
			base.Call(""{m.Name}"", {ps});" : string.Empty)}
			try {{
				{@return}{callerStr}.{m.Name}({args});
			}} catch (System.Exception e) {{
				return new System.Exception(""{m.Name}"", e);
			}}
			return null;
		}}
	}}
}}
";
			}).ToArray();
			compiler.CompileAssemblyFromSource(ops, sources);
			s = Assembly.Load(System.IO.File.ReadAllBytes(file));
			System.IO.File.Delete(file);
		}

		protected object GetTarget() {
			return target is Type t && !t.IsAbstract && t.GetConstructors().Any(e => e.GetParameters().Length == 0) ? Activator.CreateInstance(t) : target;
		}

		private bool IsTemp() {
			return GetType().Assembly.Location == string.Empty;
		}

		private string MapHashCode(string method, IEnumerable<Type> ps) {
			return method + Math.Abs(method.GetHashCode() + ps.Aggregate(0, (s, ss) => s + ss?.GetHashCode() ?? 0));
		}

		public virtual object Call(string method, params object[] ps) {
			if (IsTemp()) {
				return default;
			}
			return (Activator.CreateInstance(s.DefinedTypes.First(e => e.Name == MapHashCode(method, ps.Select(e => e?.GetType()))), target) as SafeCaller).Call(method, ps);
		}
	}
}
