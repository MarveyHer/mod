using ai.behaviours;

public class BehMadnessRandomEmotion : BehaviourActionActor
{
	private const int STATUS_DURATION = 10;

	public override BehResult execute(Actor pActor)
	{
		if (Randy.randomBool())
		{
			using (ListPool<string> tTempStatuses = new ListPool<string>())
			{
				tTempStatuses.Add("laughing");
				tTempStatuses.Add("crying");
				tTempStatuses.Add("swearing");
				string tStatusId = tTempStatuses.GetRandom();
				pActor.addStatusEffect(tStatusId, 10f, pColorEffect: false);
				return BehResult.Continue;
			}
		}
		using ListPool<string> tTempTasks = new ListPool<string>();
		tTempTasks.Add("happy_laughing");
		tTempTasks.Add("crying");
		tTempTasks.Add("swearing");
		if (tTempTasks.Count == 0)
		{
			return BehResult.Stop;
		}
		string tTaskId = tTempTasks.GetRandom();
		return forceTask(pActor, tTaskId, pClean: false, pForceAction: true);
	}
}
