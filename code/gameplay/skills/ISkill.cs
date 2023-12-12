public interface ISkill
{
	// protected int _activationPointsCost;

	// protected int _activationPoints = 0;

	public event Notification SkillReady;
	public event Notification SkillUsed;

	public abstract void Setup(SessionController sessionController);
	public abstract void Activate();
}