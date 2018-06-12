using System;

namespace Gods.Web {
	public interface IMapper {
		object MapObject(Type type, object value);
	}
}