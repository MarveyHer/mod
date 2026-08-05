public class DecisionHelper
{
	internal static UtilityBasedDecisionSystem decision_system = new UtilityBasedDecisionSystem();

	public static bool makeDecisionFor(Actor pActor, out string pLastDecisionID)
	{
		pLastDecisionID = string.Empty;
		if (pActor.isStatsDirty())
		{
			pActor.setTask("wait");
			return false;
		}
		DecisionAsset tDecisionAsset = decision_system.useOn(pActor);
		if (tDecisionAsset == null)
		{
			return false;
		}
		pLastDecisionID = tDecisionAsset.id;
		string tTaskID = tDecisionAsset.id;
		if (!string.IsNullOrEmpty(tDecisionAsset.task_id))
		{
			tTaskID = tDecisionAsset.task_id;
		}
		pActor.setTask(tTaskID);
		return true;
	}

	public static void runSimulation(Actor pActor)
	{
		decision_system.useOn(pActor, pGameplay: false);
	}

	public static void runSimulationForMindTab(Actor pActor)
	{
		decision_system.useOn(pActor, pGameplay: false);
	}
}
