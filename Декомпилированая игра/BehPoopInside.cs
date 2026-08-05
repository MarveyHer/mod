using ai.behaviours;

public class BehPoopInside : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		pActor.donePooping();
		string tBuildingID = ((!pActor.hasSubspecies()) ? "poop" : pActor.subspecies.getRandomBioProduct());
		if (tBuildingID != "poop")
		{
			BuildingHelper.tryToBuildNear(pActor.current_tile, tBuildingID);
		}
		return BehResult.Continue;
	}
}
