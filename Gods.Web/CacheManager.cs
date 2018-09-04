using System.Reflection;

namespace Gods.Web {
	public interface ICacheManager {
		int User { get; }
		int Cacheable(MethodInfo method);
		void Save(int i, object value);
		object Read(int i);
		void Remove(int i);
	}
}