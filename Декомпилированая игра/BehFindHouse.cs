using ai.behaviours;

public class BehFindHouse : BehCityActor
{
	public override BehResult execute(Actor pActor)
	{
		if (pActor.hasHouse())
		{
			return BehResult.Stop;
		}
		Building tBuilding = null;
		foreach (Building tCityBuilding in pActor.city.buildings)
		{
			if (!tCityBuilding.isUnderConstruction() && tCityBuilding.hasResidentSlots())
			{
				tBuilding = tCityBuilding;
				break;
			}
		}
		if (tBuilding == null)
		{
			tBuilding = tryToFindFamilyHouse(pActor);
		}
		if (tBuilding == null)
		{
			return BehResult.Stop;
		}
		pActor.setHomeBuilding(tBuilding);
		pActor.changeHappiness("just_found_house", tBuilding.asset.housing_happiness);
		return BehResult.Continue;
	}

	private static Building tryToFindFamilyHouse(Actor pActor)
	{
		if (!pActor.hasFamily())
		{
			return null;
		}
		int tCheckCount = 0;
		Family tFamily = pActor.family;
		foreach (Actor tFamilyMember in pActor.family.units.LoopRandom())
		{
			if (tFamilyMember == pActor)
			{
				continue;
			}
			if (++tCheckCount > 5)
			{
				break;
			}
			if (tFamilyMember.hasHouse() && tFamilyMember.city == pActor.city)
			{
				Building tBuilding = checkBuilding(tFamilyMember.home_building, tFamily);
				if (tBuilding != null)
				{
					return tBuilding;
				}
			}
		}
		return null;
	}

	private static Building checkBuilding(Building pGetHomeBuilding, Family pFamily)
	{
		foreach (long tID in pGetHomeBuilding.residents)
		{
			Actor tActor = BehaviourActionBase<Actor>.world.units.get(tID);
			if (tActor != null && tActor.isAlive() && tActor.family == pFamily)
			{
				tActor.clearHomeBuilding();
				return pGetHomeBuilding;
			}
		}
		return null;
	}
}
