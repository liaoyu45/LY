namespace BLL {
	[Gods.AOP.TargetType(typeof(ILive))]
	public interface IWork : I {
		[Gods.AOP.TargetMethod(nameof(ILive.WakeUp))]
		int GoWork();
	}
}