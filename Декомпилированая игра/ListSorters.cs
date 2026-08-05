public class ListSorters
{
	public static int sortUnitByAgeOldFirst(Actor pActor1, Actor pActor2)
	{
		return -pActor2.data.created_time.CompareTo(pActor1.data.created_time);
	}

	public static int sortUnitByAgeYoungFirst(Actor pActor1, Actor pActor2)
	{
		return pActor2.data.created_time.CompareTo(pActor1.data.created_time);
	}

	public static int sortUnitByKills(Actor pActor1, Actor pActor2)
	{
		return -pActor1.data.kills.CompareTo(pActor2.data.kills);
	}

	public static int sortUnitByRenown(Actor pActor1, Actor pActor2)
	{
		return -pActor1.data.renown.CompareTo(pActor2.data.renown);
	}

	public static int sortUnitByGoldCoins(Actor pActor1, Actor pActor2)
	{
		return -pActor1.data.money.CompareTo(pActor2.data.money);
	}

	public static int sortUnitByGender(Actor pActor1, Actor pActor2, ActorSex pTopGender)
	{
		if (pActor1.data.sex == pActor2.data.sex)
		{
			return 0;
		}
		if (pActor1.data.sex == pTopGender)
		{
			return -1;
		}
		return 1;
	}

	public static int sortUnitByStats(Actor pActor1, Actor pActor2, string pStatId)
	{
		float tValue1 = pActor1.stats.get(pStatId);
		float tValue2 = pActor2.stats.get(pStatId);
		return -tValue1.CompareTo(tValue2);
	}

	public static Actor getUnitSortedByAgeAndTraits(ListPool<Actor> pUnits, Culture pCulture)
	{
		sortUnitsSortedByAgeAndTraits(pUnits, pCulture);
		return pUnits[0];
	}

	public static void sortUnitsSortedByAgeAndTraits(ListPool<Actor> pUnits, Culture pCulture)
	{
		if (pCulture == null)
		{
			pUnits.Sort(sortUnitByAgeOldFirst);
			return;
		}
		if (pCulture.hasTrait("ultimogeniture"))
		{
			pUnits.Sort(sortUnitByAgeYoungFirst);
		}
		else
		{
			pUnits.Sort(sortUnitByAgeOldFirst);
		}
		bool num = pCulture.hasTrait("diplomatic_ascension");
		bool tWarriorAscension = pCulture.hasTrait("warriors_ascension");
		bool tGoldenRule = pCulture.hasTrait("golden_rule");
		bool tFamesCrown = pCulture.hasTrait("fames_crown");
		if (num)
		{
			pUnits.Sort((Actor a1, Actor a2) => sortUnitByStats(a1, a2, "diplomacy"));
		}
		else if (tWarriorAscension)
		{
			pUnits.Sort((Actor a1, Actor a2) => sortUnitByStats(a1, a2, "warfare"));
		}
		else if (tFamesCrown)
		{
			pUnits.Sort((Actor a1, Actor a2) => sortUnitByRenown(a1, a2));
		}
		else if (tGoldenRule)
		{
			pUnits.Sort((Actor a1, Actor a2) => sortUnitByGoldCoins(a1, a2));
		}
		bool tPatriarchy = pCulture.hasTrait("patriarchy");
		bool tMatriarchy = pCulture.hasTrait("matriarchy");
		if (tPatriarchy || tMatriarchy)
		{
			ActorSex tSex = ((!tPatriarchy) ? ActorSex.Female : ActorSex.Male);
			pUnits.Sort((Actor a1, Actor a2) => sortUnitByGender(a1, a2, tSex));
		}
	}
}
