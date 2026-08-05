using ai.behaviours;

public class BehFindLover : BehaviourActionActor
{
	public override bool shouldRetry(Actor pActor)
	{
		if (pActor.hasCity() && BehaviourActionBase<Actor>.world.cities.isLocked())
		{
			return true;
		}
		return base.shouldRetry(pActor);
	}

	public override BehResult execute(Actor pActor)
	{
		if (pActor.hasLover())
		{
			return BehResult.Stop;
		}
		Actor tResultTarget = findLoverAround(pActor);
		if (tResultTarget == null)
		{
			tResultTarget = checkCityLovers(pActor);
		}
		if (tResultTarget != null)
		{
			pActor.becomeLoversWith(tResultTarget);
		}
		return BehResult.Continue;
	}

	private Actor findLoverAround(Actor pActor)
	{
		Actor tResultTarget = null;
		foreach (Actor tPotentialLover in Finder.getUnitsFromChunk(pActor.current_tile, 1))
		{
			if (checkIfPossibleLover(pActor, tPotentialLover))
			{
				tResultTarget = tPotentialLover;
				break;
			}
		}
		return tResultTarget;
	}

	private bool checkIfPossibleLover(Actor pActor, Actor pTarget)
	{
		if (pTarget == pActor)
		{
			return false;
		}
		if (!pTarget.hasSubspecies())
		{
			return false;
		}
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (!pTarget.canFallInLoveWith(pActor))
		{
			return false;
		}
		return true;
	}

	private Actor checkCityLovers(Actor pActor)
	{
		if (!pActor.hasCity())
		{
			return null;
		}
		Actor tResultTarget = null;
		foreach (Actor tPotentialLover in pActor.city.getUnits().LoopRandom())
		{
			if (checkIfPossibleLover(pActor, tPotentialLover) && tPotentialLover.inOwnCityBorders())
			{
				tResultTarget = tPotentialLover;
				break;
			}
		}
		return tResultTarget;
	}
}
