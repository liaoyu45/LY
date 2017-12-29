using Gods.AOP;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace ConsoleApp1 {
	class EE : IValidator {
		public bool Inheritable => true;

		public void Validate(ModelBase obj, IMethodCallMessage calling, MethodBase caller) {
			System.Console.WriteLine(nameof(calling) + ":" + calling.MethodName + "-------" + nameof(caller) + ": " + caller.Name);
		}
	}
	public class Program {
		static void Main(string[] args) {
			new EE().Validate<DailyLife>();
			var w = new DailyWork();
			while (true) {
				w.OpenComputer();
				System.Console.ReadLine();
			}
		}
	}
}
namespace ConsoleApp1 {
	public class MyModelBase : ModelBaseFactory {
		protected override IModelFactory Factory { get; } = new ModelFactory();
	}

	internal class ModelFactory : IModelFactory {
		static Dictionary<Type, Type> all = new Dictionary<Type, Type> { { typeof(IThink), typeof(Unknown) }, { typeof(INothing), typeof(NNN) } };
		public Type GetType(Type type) {
			if (type == null) {
				return null;
			}
			all.TryGetValue(type, out var v);
			return v;
		}
	}

	public class Unknown : MyModelBase, IThink {
		public void Think() {
			Console.WriteLine(nameof(Think));
		}
	}

	[TargetType(typeof(INothing))]
	public interface IThink {
		[TargetMethod(nameof(INothing.None))]
		void Think();
	}

	internal interface INothing {
		void None();
	}
	class NNN : INothing {
		public NNN() {

		}
		public void None() {
			Console.WriteLine("LKJLKJLK");
		}
	}

	[TargetType(typeof(IThink))]
	public class DailyLife : MyModelBase {
		[TargetMethod(nameof(Unknown.Think))]
		public void WakeUp() {
			Console.WriteLine(nameof(WakeUp));
		}
	}
	[TargetType(typeof(DailyLife))]
	public class DailyWork : ModelBase {
		[TargetMethod(nameof(DailyLife.WakeUp))]
		internal void OpenComputer() {
			Console.WriteLine(nameof(OpenComputer));
			Console.WriteLine();
		}
	}
}
