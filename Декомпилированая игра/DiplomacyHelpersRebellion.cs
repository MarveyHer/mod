public static class DiplomacyHelpersRebellion
{
	public static void startRebellion(Actor pActor, Plot pPlot, bool pCheckForHappiness)
	{
		City tMainCity = pActor.city;
		Kingdom tOldKingdom = tMainCity.kingdom;
		if (pActor.isCityLeader())
		{
			pActor.city.removeLeader();
		}
		Kingdom tNewKingdom = tMainCity.makeOwnKingdom(pActor, pRebellion: true);
		using ListPool<City> tCitiesInRebellion = new ListPool<City>();
		tCitiesInRebellion.Add(tMainCity);
		pActor.joinCity(tMainCity);
		War tRebellionWar = null;
		foreach (War tWar in tOldKingdom.getWars())
		{
			if (tWar.isMainAttacker(tOldKingdom) && tWar.getAsset() == WarTypeLibrary.rebellion)
			{
				tRebellionWar = tWar;
				tRebellionWar.joinDefenders(tNewKingdom);
				break;
			}
		}
		if (tRebellionWar == null)
		{
			tRebellionWar = World.world.diplomacy.startWar(tOldKingdom, tNewKingdom, WarTypeLibrary.rebellion);
			if (tOldKingdom.hasAlliance())
			{
				foreach (Kingdom tKingdom in tOldKingdom.getAlliance().kingdoms_hashset)
				{
					if (tKingdom != tOldKingdom && tKingdom.isOpinionTowardsKingdomGood(tOldKingdom))
					{
						tRebellionWar.joinAttackers(tKingdom);
					}
				}
			}
		}
		foreach (Actor unit in pPlot.units)
		{
			City tCity = unit.city;
			if (tCity != null && tCity.kingdom != tNewKingdom && tCity.kingdom == tOldKingdom)
			{
				tCity.joinAnotherKingdom(tNewKingdom, pCaptured: false, pRebellion: true);
			}
		}
		int tCurCities = tOldKingdom.countCities();
		int tMaxCitiesNew = tNewKingdom.getMaxCities();
		tMaxCitiesNew -= tCitiesInRebellion.Count;
		if (tMaxCitiesNew < 0)
		{
			tMaxCitiesNew = 0;
		}
		if (tMaxCitiesNew > tCurCities / 3)
		{
			tMaxCitiesNew = (int)((float)tCurCities / 3f);
		}
		for (int i = 0; i < tMaxCitiesNew; i++)
		{
			if (!checkMoreAlignedCities(tNewKingdom, tOldKingdom, tCitiesInRebellion, pCheckForHappiness))
			{
				break;
			}
		}
	}

	public static bool checkMoreAlignedCities(Kingdom pNewKingdom, Kingdom pOldKingdom, ListPool<City> pNewCities, bool pCheckForHappiness)
	{
		using ListPool<City> tTempCities = new ListPool<City>(World.world.cities.Count);
		addNeighbourCities(tTempCities, pNewCities);
		if (tTempCities.Count == 0)
		{
			tTempCities.AddRange(pOldKingdom.getCities());
		}
		if (tTempCities.Count == 0)
		{
			return false;
		}
		foreach (City tCity in tTempCities.LoopRandom())
		{
			if (tCity.kingdom == pOldKingdom && !tCity.isCapitalCity() && (!pCheckForHappiness || !tCity.isHappy()) && !Randy.randomBool())
			{
				tCity.joinAnotherKingdom(pNewKingdom, pCaptured: false, pRebellion: true);
				return true;
			}
		}
		return true;
	}

	private static void addNeighbourCities(ListPool<City> pTempCitiesToCheck, ListPool<City> pRebelledCities)
	{
		foreach (ref City pRebelledCity in pRebelledCities)
		{
			pRebelledCity.recalculateNeighbourCities();
		}
		foreach (ref City pRebelledCity2 in pRebelledCities)
		{
			City tCity = pRebelledCity2;
			pTempCitiesToCheck.AddRange(tCity.neighbours_cities);
		}
	}
}
