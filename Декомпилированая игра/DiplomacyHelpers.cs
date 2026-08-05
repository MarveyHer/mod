public static class DiplomacyHelpers
{
	public static WarManager wars => World.world.wars;

	public static DiplomacyManager diplomacy => World.world.diplomacy;

	public static bool isWarNeeded(Kingdom pKingdom)
	{
		if (!pKingdom.hasCities())
		{
			return false;
		}
		if (!pKingdom.hasCapital())
		{
			return false;
		}
		if (pKingdom.data.timestamp_last_war != -1.0 && Date.getYearsSince(pKingdom.data.timestamp_last_war) <= SimGlobals.m.diplomacy_years_war_timeout)
		{
			return false;
		}
		if (wars.hasWars(pKingdom))
		{
			return false;
		}
		if (pKingdom.countTotalWarriors() <= SimGlobals.m.diplomacy_years_war_min_warriors)
		{
			return false;
		}
		float tCurPopulation = pKingdom.getPopulationPeople();
		float tPopulationMax = pKingdom.getPopulationTotalPossible();
		if (pKingdom.countCities() < 4 && tCurPopulation < tPopulationMax * 0.6f)
		{
			return false;
		}
		return true;
	}

	public static Kingdom getWarTarget(Kingdom pInitiatorKingdom)
	{
		Kingdom tBestTarget = null;
		float tBestFastDist = float.MaxValue;
		int tCurrentArmy = pInitiatorKingdom.countTotalWarriors();
		if (pInitiatorKingdom.hasAlliance())
		{
			tCurrentArmy = pInitiatorKingdom.getAlliance().countWarriors();
		}
		using ListPool<Kingdom> tPossibleKingdomsList = wars.getNeutralKingdoms(pInitiatorKingdom);
		foreach (ref Kingdom item in tPossibleKingdomsList)
		{
			Kingdom tTargetKingdom = item;
			if (!tTargetKingdom.hasCities() || !tTargetKingdom.hasCapital() || tTargetKingdom.getAge() < SimGlobals.m.minimum_kingdom_age_for_attack)
			{
				continue;
			}
			int tTargetArmy = 0;
			tTargetArmy = ((!tTargetKingdom.hasAlliance()) ? tTargetKingdom.countTotalWarriors() : tTargetKingdom.getAlliance().countWarriors());
			if (tCurrentArmy >= tTargetArmy && pInitiatorKingdom.capital.reachableFrom(tTargetKingdom.capital) && !((float)Date.getYearsSince(diplomacy.getRelation(pInitiatorKingdom, tTargetKingdom).data.timestamp_last_war_ended) < (float)SimGlobals.m.minimum_years_between_wars) && !pInitiatorKingdom.isOpinionTowardsKingdomGood(tTargetKingdom))
			{
				float tFastDist = Kingdom.distanceBetweenKingdom(pInitiatorKingdom, tTargetKingdom);
				if (tFastDist < tBestFastDist)
				{
					tBestFastDist = tFastDist;
					tBestTarget = tTargetKingdom;
				}
			}
		}
		return tBestTarget;
	}

	public static Kingdom getAllianceTarget(Kingdom pKingdomStarter)
	{
		if (pKingdomStarter.isSupreme())
		{
			return null;
		}
		using ListPool<Kingdom> tKingdoms = World.world.wars.getNeutralKingdoms(pKingdomStarter, pOnlyWithoutWars: true, pOnlyWithoutAlliances: true);
		if (tKingdoms.Count == 0)
		{
			return null;
		}
		foreach (Kingdom tKingdom in tKingdoms.LoopRandom())
		{
			if (tKingdom.hasKing() && !tKingdom.isSupreme() && !tKingdom.king.hasPlot() && pKingdomStarter.isOpinionTowardsKingdomGood(tKingdom) && tKingdom.getRenown() >= PlotsLibrary.alliance_create.min_renown_kingdom)
			{
				bool tGoodKingdomTarget = false;
				if (pKingdomStarter.countCities() <= 2 && tKingdom.countCities() <= 2 && !pKingdomStarter.hasNearbyKingdoms() && !tKingdom.hasNearbyKingdoms())
				{
					tGoodKingdomTarget = true;
				}
				if (!tGoodKingdomTarget && areKingdomsClose(tKingdom, pKingdomStarter))
				{
					tGoodKingdomTarget = true;
				}
				if (tGoodKingdomTarget)
				{
					return tKingdom;
				}
			}
		}
		return null;
	}

	public static bool areKingdomsClose(Kingdom pMain, Kingdom pTarget)
	{
		foreach (City tCityMain in pMain.getCities())
		{
			foreach (City tCityTarget in pTarget.getCities())
			{
				if (City.nearbyBorders(tCityMain, tCityTarget))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool isThereActiveCityConquest(Kingdom pKingdom, Kingdom pTargetKingdom)
	{
		foreach (City city in pKingdom.getCities())
		{
			if (city.isGettingCapturedBy(pTargetKingdom))
			{
				return true;
			}
		}
		foreach (City city2 in pTargetKingdom.getCities())
		{
			if (city2.isGettingCapturedBy(pKingdom))
			{
				return true;
			}
		}
		return false;
	}

	public static bool isThereFightBetween(Kingdom pKingdom1, Kingdom pKingdom2)
	{
		if (isThereActiveCityFight(pKingdom1, pKingdom2))
		{
			return true;
		}
		if (isThereActiveCityFight(pKingdom2, pKingdom1))
		{
			return true;
		}
		return false;
	}

	private static bool isThereActiveCityFight(Kingdom pDefenderKingdom, Kingdom pAttackerKingdom)
	{
		foreach (City tCity in pDefenderKingdom.getCities())
		{
			if (!tCity.hasArmy())
			{
				continue;
			}
			Army tArmy = tCity.army;
			if (tArmy.hasCaptain())
			{
				Actor tArmyLeader = tArmy.getCaptain();
				if (tArmyLeader.current_tile.hasCity() && tArmyLeader.current_tile.zone_city.kingdom == pAttackerKingdom)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool areDefendersGettingCaptured(this War pWar)
	{
		foreach (Kingdom defender in pWar.getDefenders())
		{
			if (defender.isGettingCaptured())
			{
				return true;
			}
		}
		return false;
	}

	public static bool areAttackersGettingCaptured(this War pWar)
	{
		foreach (Kingdom attacker in pWar.getAttackers())
		{
			if (attacker.isGettingCaptured())
			{
				return true;
			}
		}
		return false;
	}

	public static bool areAttackersAttackingAnotherCity(this War pWar)
	{
		foreach (Kingdom attacker in pWar.getAttackers())
		{
			if (attacker.isAttackingAnotherCity())
			{
				return true;
			}
		}
		return false;
	}

	public static bool areDefendersAttackingAnotherCity(this War pWar)
	{
		foreach (Kingdom defender in pWar.getDefenders())
		{
			if (defender.isAttackingAnotherCity())
			{
				return true;
			}
		}
		return false;
	}

	public static bool isAttackingAnotherCity(this Kingdom pAttackerKingdom)
	{
		foreach (City tCity in pAttackerKingdom.getCities())
		{
			if (!tCity.hasArmy())
			{
				continue;
			}
			Army tArmy = tCity.army;
			if (tArmy.hasCaptain())
			{
				Actor tArmyLeader = tArmy.getCaptain();
				if (tArmyLeader.current_tile.hasCity() && tArmyLeader.current_tile.zone_city.kingdom.isEnemy(pAttackerKingdom))
				{
					return true;
				}
			}
		}
		return false;
	}
}
