using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace Gods.Web {
	public class CacheManager : ICacheManager {
		class Cache {
			public long SetTime { get; set; }
			public int Key { get; set; }
			public object Value { get; set; }
		}
		internal static CacheManager Instance = new CacheManager();
		public virtual int User => HttpContext.Current.Session?[nameof(User)]?.GetHashCode() ?? 0;

		private static List<Cache> cache = new List<Cache>();
		private static Thread timer;
		static CacheManager() {
			timer = new Thread(() => {
				while (true) {
					Thread.Sleep(10000);
					//cache.Where(e => DateTime.Now.Ticks - e.SetTime > 1000 * 3600).ToList().ForEach(e => cache.Remove(e));
				}
			});
			timer.Start();
		}
		~CacheManager() {
			timer.Abort();
		}

		public int Cacheable(MethodInfo method) {
			return Gods.Him.GetAllAttributes<CacheAttribute>(method).FirstOrDefault()?.Id ?? 0;
		}

		public void Remove(int i) {
			cache.Remove(cache.FirstOrDefault(e => e.Key == -i));
		}

		public object Read(int i) {
			return cache.FirstOrDefault(e => e.Key == i);
		}

		public void Save(int i, object value) {
			cache.Add(new Cache { Key = i, Value = value, SetTime = DateTime.Now.Ticks });
		}
	}
	public interface ICacheManager {
		int User { get; }
		int Cacheable(MethodInfo method);
		void Save(int i, object value);
		object Read(int i);
		void Remove(int i);
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class CacheAttribute : Attribute {

		public CacheAttribute(int id) {
			Id = id;
		}

		public int Id { get; private set; }
	}
}