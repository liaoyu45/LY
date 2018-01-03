namespace BLL {
	[Gods.AOP.TargetType(typeof(ILive))]
	public interface IWork {
		[Gods.AOP.TargetMethod(nameof(ILive.WakeUp))]
		int GoWork();
	}
}