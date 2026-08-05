using ai.behaviours;

public class BehFindTargetToStealFrom : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		Actor tTarget = getClosestActorWithMoneys(pActor);
		if (tTarget == null)
		{
			return BehResult.Stop;
		}
		pActor.beh_actor_target = tTarget;
		return BehResult.Continue;
	}

	private Actor getClosestActorWithMoneys(Actor pActor)
	{
		using ListPool<Actor> tTempActors = new ListPool<Actor>(4);
		bool tRandomShuffle = Randy.randomBool();
		int tChunkRange = Randy.randomInt(1, 4);
		int tMaxUnits = Randy.randomInt(1, 4);
		foreach (Actor tTarget in Finder.getUnitsFromChunk(pActor.current_tile, tChunkRange, 0f, tRandomShuffle))
		{
			if (tTarget != pActor && pActor.isSameIslandAs(tTarget) && tTarget.hasAnyCash())
			{
				tTempActors.Add(tTarget);
				if (tTempActors.Count >= tMaxUnits)
				{
					break;
				}
			}
		}
		return Toolbox.getClosestActor(tTempActors, pActor.current_tile);
	}
}
