using System;

namespace Gods.AOP {
	public interface IMapper {
		object MapObject(Type type, object value);
	}
}