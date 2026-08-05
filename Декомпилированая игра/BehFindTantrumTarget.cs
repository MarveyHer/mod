using ai.behaviours;

public class BehFindTantrumTarget : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		if (pActor.beh_actor_target != null && pActor.isTargetOkToAttack(pActor.beh_actor_target.a))
		{
			return BehResult.Continue;
		}
		Actor tTarget = getClosestActor(pActor);
		if (tTarget == null)
		{
			return forceTask(pActor, "random_move");
		}
		pActor.beh_actor_target = tTarget;
		return BehResult.Continue;
	}

	private Actor getClosestActor(Actor pActor)
	{
		bool tRandomShuffle = Randy.randomBool();
		WorldTile tTile = pActor.current_tile;
		float tBestDist = 2.1474836E+09f;
		Actor tBestActor = null;
		foreach (Actor tTargetToCheck in Finder.getUnitsFromChunk(tTile, 1, 0f, tRandomShuffle))
		{
			float tDist = Toolbox.SquaredDistTile(tTargetToCheck.current_tile, tTile);
			if (!(tDist >= tBestDist) && pActor.isTargetOkToAttack(tTargetToCheck) && (!tTargetToCheck.hasStatusStunned() || pActor.areFoes(tTargetToCheck)))
			{
				tBestDist = tDist;
				tBestActor = tTargetToCheck;
				if (Randy.randomBool())
				{
					break;
				}
			}
		}
		return tBestActor;
	}
}
