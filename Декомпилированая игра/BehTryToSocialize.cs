using ai.behaviours;

public class BehTryToSocialize : BehaviourActionActor
{
	public override BehResult execute(Actor pActor)
	{
		pActor.resetSocialize();
		Actor tTarget = getRandomActorAround(pActor);
		if (tTarget != null)
		{
			pActor.beh_actor_target = tTarget;
			if (pActor.canFallInLoveWith(tTarget))
			{
				pActor.becomeLoversWith(tTarget);
			}
			pActor.resetSocialize();
			tTarget.resetSocialize();
			if (pActor.hasTelepathicLink() && tTarget.hasTelepathicLink())
			{
				return forceTask(pActor, "socialize_do_talk", pClean: false);
			}
			return forceTask(pActor, "socialize_go_to_target", pClean: false);
		}
		return BehResult.Stop;
	}

	private Actor getRandomActorAround(Actor pActor)
	{
		using ListPool<Actor> tBestTargets = new ListPool<Actor>(4);
		using ListPool<Actor> tAllTargets = new ListPool<Actor>(4);
		bool tNeedOppositeSex = pActor.subspecies.needOppositeSexTypeForReproduction();
		bool tHasAnimalWhispererTrait = pActor.hasCulture() && pActor.culture.hasTrait("animal_whisperers");
		bool num = pActor.hasTelepathicLink();
		if (num)
		{
			fillUnitsViaTelepathicLink(pActor, tBestTargets, tAllTargets);
		}
		int tChunkRange = 1;
		if (num)
		{
			tChunkRange = 2;
		}
		foreach (Actor tSocializeTarget in Finder.getUnitsFromChunk(pActor.current_tile, tChunkRange, 0f, pRandom: true))
		{
			if (!pActor.canTalkWith(tSocializeTarget))
			{
				continue;
			}
			if (pActor.isKingdomCiv())
			{
				if (tSocializeTarget.isKingdomMob())
				{
					if (!tHasAnimalWhispererTrait)
					{
						continue;
					}
				}
				else if (!tSocializeTarget.isKingdomCiv())
				{
				}
			}
			else if (!pActor.isSameSpecies(tSocializeTarget))
			{
				continue;
			}
			if (tNeedOppositeSex && pActor.canFallInLoveWith(tSocializeTarget))
			{
				tBestTargets.Add(tSocializeTarget);
				break;
			}
			tAllTargets.Add(tSocializeTarget);
			if (tAllTargets.Count > 3)
			{
				break;
			}
		}
		if (tBestTargets.Count > 0)
		{
			return tBestTargets.GetRandom();
		}
		if (tAllTargets.Count > 0)
		{
			return tAllTargets.GetRandom();
		}
		return null;
	}

	private void fillUnitsViaTelepathicLink(Actor pActor, ListPool<Actor> pBestTargets, ListPool<Actor> pNormalTargets)
	{
		if (pActor.hasFamily())
		{
			foreach (Actor tActor in pActor.family.units)
			{
				if (pActor.canTalkWith(tActor))
				{
					pNormalTargets.Add(tActor);
				}
			}
		}
		foreach (Actor tParent in pActor.getParents())
		{
			if (pActor.canTalkWith(tParent))
			{
				pBestTargets.Add(tParent);
			}
		}
	}
}
