using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gods.Module {
	public class Loader : IDisposable {
		private readonly List<object> allInstances = new List<object>();

		public List<Type> AllTypes { get; private set; }

		public void LoadFromFolder(string folder) =>
			LoadFromFolder(folder, true);

		public void LoadFromFolder(string folder, bool occupy) {
			var v = GetType().Assembly.FullName.Split(',')[0];
			AllTypes = Directory.GetFiles(folder, "*.dll").SelectMany(f => {
				if (v == f.Split('\\', '/').Last().Split('.')[0]) {
					return Type.EmptyTypes;
				}
				try {
					return (occupy ? Assembly.LoadFrom(f) : Assembly.Load(File.ReadAllBytes(f))).GetTypes().Where(t => !t.IsInterface && !t.IsGenericType && !t.IsAbstract);
				} catch {
					return Type.EmptyTypes;
				}
			}).ToList();
		}

		public T Instance<T>() =>
			Instances<T>(false, t => true).FirstOrDefault();

		public IEnumerable<T> Instances<T>() =>
			Instances<T>(false, t => true);

		public T Instance<T>(bool createNew, Func<T, bool> filter) =>
			Instances(createNew, filter).FirstOrDefault();

		public IEnumerable<T> Instances<T>(bool createNew, Func<T, bool> filter) {
			var bt = typeof(T);
			var ii = bt.IsInterface;
			return from type in AllTypes
				   where (ii ? type.GetInterfaces().Contains(bt) : type.IsSubclassOf(bt))
				   let r = (T)create(type, createNew)
				   where filter(r)
				   orderby type.GUID
				   select r;
		}

		private object create(Type type, bool createNew) {
			var r = allInstances.FirstOrDefault(i => i.GetType() == type) ?? Activator.CreateInstance(type);
			if (createNew) {
				allInstances.Remove(r);
			}
			allInstances.Add(r);
			return r;
		}

		public void Dispose() {
			foreach (var item in allInstances.OfType<IDisposable>()) {
				item.Dispose();
			}
			allInstances.Clear();
		}

		public object Reload(object obj) {
			if (allInstances.Contains(obj)) {
				allInstances.Remove(obj);
			}
			(obj as IDisposable)?.Dispose();
			obj = Activator.CreateInstance(obj?.GetType());
			allInstances.Add(obj);
			return obj;
		}
	}
}