using ai.behaviours;

public class BehCopyAggro : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		Actor tTarget = pActor.beh_actor_target?.a;
		if (tTarget == null)
		{
			return BehResult.Continue;
		}
		pActor.copyAggroFrom(tTarget);
		copyEnemiesOf(pActor, tTarget);
		return BehResult.Continue;
	}

	private void copyEnemiesOf(Actor pCopyTo, Actor pTarget)
	{
		foreach (Actor tPossibleEnemy in Finder.getUnitsFromChunk(pTarget.current_tile, 1, 0f, pRandom: true))
		{
			if (tPossibleEnemy != pCopyTo && tPossibleEnemy.isInAggroList(pTarget) && pCopyTo.isSameIslandAs(tPossibleEnemy))
			{
				pCopyTo.addAggro(tPossibleEnemy);
			}
		}
	}
}
