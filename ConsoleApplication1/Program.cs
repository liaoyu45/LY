using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
	class Program {
		[Import]
		public World.ILimit<Me.I> MyProperty { get; set; }
		static void Main(string[] args) {
			var f = new AssemblyCatalog(Assembly.LoadFrom(@"C:\Users\liaoyu45\Source\Repos\LY\Me.Limit\bin\Debug\Me.Limit.dll"));
			var c = new CompositionContainer(
				f
				);
			var program = new Program();
			c.ComposeExportedValue("i", 1);
			c.ComposeParts(program);
			Console.WriteLine(program);
		}
	}
	[Export(typeof(World.ILimit<Me.I>))]
	public class MyClass : World.ILimit<Me.I> {
		[ImportingConstructor]
		public MyClass([Import("i")]int i) {
			Console.WriteLine(i);
		}
	}
}
