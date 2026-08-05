using UnityEngine;

public static class BabyHelper
{
	public static Actor debugTryToMakeUnit(Actor pActor)
	{
		WorldTile current_tile = pActor.current_tile;
		Actor tTarget = null;
		foreach (Actor tActor in Finder.getUnitsFromChunk(current_tile, 1, 10f))
		{
			if (tActor != pActor && tActor.subspecies == pActor.subspecies)
			{
				tTarget = tActor;
				break;
			}
		}
		if (tTarget == null)
		{
			return null;
		}
		return BabyMaker.makeBaby(pActor, tTarget);
	}

	public static void countBirth(Actor pBaby)
	{
		World.world.game_stats.data.creaturesBorn++;
		World.world.map_stats.creaturesBorn++;
		if (pBaby.hasCity())
		{
			pBaby.city.increaseBirths();
		}
		if (pBaby.hasClan())
		{
			pBaby.clan.increaseBirths();
		}
		if (pBaby.hasFamily())
		{
			pBaby.family.increaseBirths();
		}
		if (pBaby.hasSubspecies())
		{
			pBaby.subspecies.increaseBirths();
		}
		if (pBaby.isKingdomCiv())
		{
			pBaby.kingdom.increaseBirths();
		}
	}

	public static void applyParentsMeta(Actor pParent1, Actor pParent2, Actor pBaby)
	{
		Subspecies tBabySubspecies = getBabySubspecies(pParent1, pParent2);
		pBaby.setSubspecies(tBabySubspecies);
		Family tFamily = pParent1.family;
		Clan tClan = checkGreatClan(pParent1, pParent2);
		if (tClan != null && !tClan.isFull())
		{
			pBaby.setClan(tClan);
		}
		if (tBabySubspecies.isSapient())
		{
			if (pParent1.hasCity())
			{
				pBaby.setCity(pParent1.city);
			}
			else if (pParent2 != null && pParent2.hasCity())
			{
				pBaby.setCity(pParent2.city);
			}
		}
		if (tFamily != null)
		{
			pBaby.setFamily(tFamily);
			pBaby.saveOriginFamily(tFamily.data.id);
		}
		using ListPool<Culture> tPotCultures = new ListPool<Culture>(2);
		using ListPool<Religion> tPotReligions = new ListPool<Religion>(2);
		using ListPool<Language> tPotLanguages = new ListPool<Language>(2);
		using ListPool<int> tPotPhenotypes = new ListPool<int>(2);
		tPotPhenotypes.Add(pParent1.data.phenotype_index);
		if (pParent1.hasCulture())
		{
			tPotCultures.Add(pParent1.culture);
		}
		if (pParent1.hasReligion())
		{
			tPotReligions.Add(pParent1.religion);
		}
		if (pParent1.hasLanguage())
		{
			tPotLanguages.Add(pParent1.language);
		}
		if (pParent2 != null)
		{
			if (pParent2.hasCulture())
			{
				tPotCultures.Add(pParent2.culture);
			}
			if (pParent2.hasReligion())
			{
				tPotReligions.Add(pParent2.religion);
			}
			if (pParent2.hasLanguage())
			{
				tPotLanguages.Add(pParent2.language);
			}
			if (pParent2.subspecies == pBaby.subspecies)
			{
				tPotPhenotypes.Add(pParent2.data.phenotype_index);
			}
		}
		if (tPotCultures.Count > 0 && tBabySubspecies.has_advanced_memory)
		{
			pBaby.setCulture(tPotCultures.GetRandom());
		}
		if (tPotReligions.Count > 0 && tBabySubspecies.has_advanced_memory)
		{
			pBaby.setReligion(tPotReligions.GetRandom());
		}
		if (tPotLanguages.Count > 0 && tBabySubspecies.has_advanced_communication)
		{
			pBaby.joinLanguage(tPotLanguages.GetRandom());
		}
		if (pParent1 != null && pParent1.hasCultureTrait("ancestors_knowledge"))
		{
			string tBestAttribute = getBestAtribute(pParent1);
			if (tBestAttribute != null)
			{
				pBaby.data[tBestAttribute] = (float)(int)pParent1.data[tBestAttribute] * 0.5f + 1f;
			}
		}
		if (pParent2 != null && pParent2.hasCultureTrait("ancestors_knowledge"))
		{
			string tBestAttribute2 = getBestAtribute(pParent2);
			if (tBestAttribute2 != null)
			{
				pBaby.data[tBestAttribute2] = (float)(int)pParent2.data[tBestAttribute2] * 0.5f + 1f;
			}
		}
		pBaby.data.phenotype_index = tPotPhenotypes.GetRandom();
		pBaby.data.phenotype_shade = Actor.getRandomPhenotypeShade();
		if (tBabySubspecies.hasTrait("parental_care"))
		{
			pBaby.addStatusEffect("invincible", 90f);
		}
	}

	private static string getBestAtribute(Actor pParent1)
	{
		string tBestAttribute = null;
		int tBestValue = 0;
		if (pParent1.data["intelligence"] > (float)tBestValue)
		{
			tBestValue = (int)pParent1.data["intelligence"];
			tBestAttribute = "intelligence";
		}
		if (pParent1.data["warfare"] > (float)tBestValue)
		{
			tBestValue = (int)pParent1.data["warfare"];
			tBestAttribute = "warfare";
		}
		if (pParent1.data["diplomacy"] > (float)tBestValue)
		{
			tBestValue = (int)pParent1.data["diplomacy"];
			tBestAttribute = "diplomacy";
		}
		if (pParent1.data["stewardship"] > (float)tBestValue)
		{
			tBestValue = (int)pParent1.data["stewardship"];
			tBestAttribute = "stewardship";
		}
		return tBestAttribute;
	}

	private static Clan checkGreatClan(Actor pParent1, Actor pParent2)
	{
		Clan tClan = null;
		if (pParent1.isKing())
		{
			tClan = pParent1.clan;
		}
		else if (pParent2 != null && pParent2.isKing())
		{
			tClan = pParent2.clan;
		}
		if (tClan == null)
		{
			if (pParent1.isCityLeader() && pParent2 != null && pParent2.isCityLeader())
			{
				tClan = ((!Randy.randomBool()) ? pParent2.clan : pParent1.clan);
			}
			else if (pParent1 != null && pParent1.isCityLeader())
			{
				tClan = pParent1.clan;
			}
			else if (pParent2 != null && pParent2.isCityLeader())
			{
				tClan = pParent2.clan;
			}
		}
		return tClan;
	}

	private static Subspecies getBabySubspecies(Actor pParent1, Actor pParent2)
	{
		Subspecies tSubspecies1 = pParent1.subspecies;
		Subspecies tSubspecies2 = pParent2?.subspecies ?? tSubspecies1;
		if (tSubspecies1.isSapient() && tSubspecies1.isSapient() != tSubspecies2.isSapient())
		{
			if (tSubspecies1.isSapient())
			{
				return tSubspecies1;
			}
			return tSubspecies2;
		}
		if (tSubspecies1 != tSubspecies2 && tSubspecies1.getGeneration() != tSubspecies2.getGeneration())
		{
			if (tSubspecies1.getGeneration() > tSubspecies2.getGeneration())
			{
				return tSubspecies1;
			}
			return tSubspecies2;
		}
		if (Randy.randomBool())
		{
			return tSubspecies1;
		}
		return tSubspecies2;
	}

	public static bool canMakeBabies(Actor pActor)
	{
		if (!pActor.isAdult())
		{
			return false;
		}
		if (!pActor.canProduceBabies())
		{
			return false;
		}
		if (pActor.hasReachedOffspringLimit())
		{
			return false;
		}
		if (!pActor.haveNutritionForNewBaby())
		{
			return false;
		}
		return true;
	}

	public static bool isMetaLimitsReached(Actor pActor)
	{
		if (pActor.subspecies.hasReachedPopulationLimit())
		{
			return true;
		}
		if (pActor.hasCity())
		{
			if (pActor.city.hasReachedWorldLawLimit())
			{
				return true;
			}
			Actor tLover = pActor.lover;
			bool num = pActor.isImportantPerson() && !pActor.hasReachedOffspringLimit();
			bool tLoverImportant = tLover != null && tLover.isImportantPerson() && !tLover.hasReachedOffspringLimit();
			if (num || tLoverImportant)
			{
				return false;
			}
			if (pActor.subspecies.isReproductionSexual() && pActor.current_children_count == 0)
			{
				return false;
			}
			if (!pActor.city.hasFreeHouseSlots())
			{
				return true;
			}
		}
		return false;
	}

	public static void countMakeChild(Actor pParent1, Actor pParent2)
	{
		if (!pParent1.isRekt())
		{
			pParent1.increaseBirths();
		}
		if (!pParent2.isRekt())
		{
			pParent2.increaseBirths();
		}
	}

	public static void babyMakingStart(Actor pActor)
	{
		pActor.subspecies.all_actions_actor_birth?.Invoke(pActor, pActor.current_tile);
	}

	public static void traitsClone(Actor pActorTarget, Actor pParent1)
	{
		foreach (ActorTrait tParentTrait in pParent1.getTraits())
		{
			if (tParentTrait.rate_birth != 0 || tParentTrait.rate_inherit != 0)
			{
				pActorTarget.addTrait(tParentTrait);
			}
		}
	}

	public static void traitsInherit(Actor pActorTarget, Actor pParent1, Actor pParent2)
	{
		using ListPool<ActorTrait> tPossibleTraits = new ListPool<ActorTrait>(128);
		int tTotalParentTraits1 = 0;
		int tTotalParentTraits2 = 0;
		addTraitsFromParentToList(pParent1, tPossibleTraits, out tTotalParentTraits1);
		if (pParent2 != null)
		{
			addTraitsFromParentToList(pParent2, tPossibleTraits, out tTotalParentTraits2);
		}
		if (tPossibleTraits.Count != 0)
		{
			int tTotalParentTraits3 = (int)((float)(tTotalParentTraits1 + tTotalParentTraits2) * 0.25f);
			tTotalParentTraits3 = Mathf.Max(1, tTotalParentTraits3);
			for (int i = 0; i < tTotalParentTraits3; i++)
			{
				ActorTrait tTrait = tPossibleTraits.GetRandom();
				pActorTarget.addTrait(tTrait.id);
			}
		}
	}

	private static void addTraitsFromParentToList(Actor pActor, ListPool<ActorTrait> pList, out int pCounter)
	{
		int tResultCounter = 0;
		foreach (ActorTrait tTrait in pActor.getTraits())
		{
			if (tTrait.rate_inherit != 0 || tTrait.rate_birth != 0)
			{
				tResultCounter++;
				pList.AddTimes(tTrait.rate_birth, tTrait);
				pList.AddTimes(tTrait.rate_inherit, tTrait);
			}
		}
		pCounter = tResultCounter;
	}
}
