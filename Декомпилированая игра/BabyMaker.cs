using UnityEngine;

public class BabyMaker
{
	public static void startMiracleBirth(Actor pActor)
	{
		BabyHelper.babyMakingStart(pActor);
		if (pActor.hasSubspeciesTrait("reproduction_strategy_viviparity") && pActor.isSexFemale())
		{
			pActor.addStatusEffect("pregnant", pActor.getMaturationTimeSeconds());
		}
		else
		{
			pActor.birthEvent("miracle_bearer");
			makeBabyFromMiracle(pActor, ActorSex.Male, pAddToFamily: true);
			makeBabyFromMiracle(pActor, ActorSex.Female, pAddToFamily: true);
			if (Randy.randomBool())
			{
				makeBabyFromMiracle(pActor, ActorSex.None, pAddToFamily: true);
			}
		}
		pActor.subspecies.counterReproduction();
	}

	public static void startSoulborneBirth(Actor pActor)
	{
		BabyHelper.babyMakingStart(pActor);
		if (pActor.subspecies.hasTrait("reproduction_strategy_viviparity") && pActor.isSexFemale())
		{
			pActor.addStatusEffect("pregnant", pActor.getMaturationTimeSeconds());
		}
		else
		{
			pActor.birthEvent();
			makeBaby(pActor, null, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: false, pJoinFamily: true);
		}
		pActor.subspecies.counterReproduction();
	}

	public static void spawnSporesFor(Actor pActor)
	{
		pActor.birthEvent();
		BabyHelper.babyMakingStart(pActor);
		int tSporesAmount = Randy.randomInt(3, 10);
		for (int i = 0; i < tSporesAmount; i++)
		{
			Spores tSpores = (Spores)EffectsLibrary.spawn("fx_spores", pActor.current_tile);
			if (tSpores == null)
			{
				return;
			}
			tSpores.prepare();
			tSpores.setActorParent(pActor);
		}
		pActor.subspecies.counterReproduction();
	}

	public static void spawnBabyFromSpore(Actor pActor, Vector3 pPosition)
	{
		WorldTile tTile = World.world.GetTile((int)pPosition.x, (int)pPosition.y);
		if (tTile != null)
		{
			makeBaby(pActor, null, ActorSex.None, pCloneTraits: false, 0, tTile, pAddToFamily: false, pJoinFamily: true);
		}
	}

	public static void makeBabyFromMiracle(Actor pActor, ActorSex pSex = ActorSex.None, bool pAddToFamily = false)
	{
		makeBaby(pActor, null, pSex, pCloneTraits: false, 0, null, pAddToFamily).addTrait("miracle_born");
	}

	public static Actor makeBabyViaFission(Actor pActor)
	{
		pActor.birthEvent();
		BabyHelper.babyMakingStart(pActor);
		Actor actor = makeBaby(pActor, null, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: false, pJoinFamily: true);
		int tParentHealth = pActor.getHealth() / 2;
		int tParentHappiness = pActor.getHappiness() / 2;
		int tParentNutrition = pActor.getNutrition() / 2;
		pActor.setHealth(tParentHealth);
		pActor.setStamina(0);
		pActor.setHappiness(tParentHappiness);
		pActor.setNutrition(tParentNutrition);
		actor.setHealth(tParentHealth);
		actor.setStamina(0);
		actor.setHappiness(tParentHappiness);
		actor.setNutrition(tParentNutrition);
		pActor.subspecies.counterReproduction();
		return actor;
	}

	public static Actor makeBabyViaBudding(Actor pActor)
	{
		pActor.birthEvent();
		BabyHelper.babyMakingStart(pActor);
		return makeBaby(pActor, null, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: false, pJoinFamily: true);
	}

	public static Actor makeBabyViaVegetative(Actor pActor)
	{
		pActor.birthEvent();
		BabyHelper.babyMakingStart(pActor);
		Actor tBaby = makeBaby(pActor, null, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: false, pJoinFamily: true);
		tBaby.addStatusEffect("uprooting", tBaby.getMaturationTimeSeconds());
		return tBaby;
	}

	public static void makeBabyViaParthenogenesis(Actor pActor)
	{
		pActor.birthEvent();
		BabyHelper.babyMakingStart(pActor);
		makeBaby(pActor, null, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: false, pJoinFamily: true);
		pActor.subspecies.counterReproduction();
	}

	public static void makeBabiesViaSexual(Actor pMotherTarget, Actor pParentA, Actor pParentB)
	{
		pParentA.birthEvent();
		pParentB.birthEvent();
		BabyHelper.babyMakingStart(pParentA);
		BabyHelper.babyMakingStart(pParentB);
		newImmediateBabySpawn(pParentA, pParentB);
		int tMaxBonusBabies = (int)pMotherTarget.stats["birth_rate"];
		float tChance = 0.5f;
		for (int i = 0; i < tMaxBonusBabies; i++)
		{
			if (!Randy.randomChance(tChance))
			{
				break;
			}
			newImmediateBabySpawn(pParentA, pParentB);
			tChance *= 0.85f;
		}
	}

	public static void makeBabyFromPregnancy(Actor pActor)
	{
		pActor.hasLover();
		Actor tLover = pActor.lover;
		pActor.birthEvent();
		makeBaby(pActor, tLover, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: true);
		float tChance = 0.5f;
		int tMaxBonusBabies = (int)pActor.stats["birth_rate"];
		for (int i = 0; i < tMaxBonusBabies; i++)
		{
			if (!Randy.randomChance(tChance))
			{
				break;
			}
			makeBaby(pActor, tLover, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: true);
			tChance *= 0.85f;
		}
	}

	private static void newImmediateBabySpawn(Actor pParent1, Actor pParent2)
	{
		makeBaby(pParent1, pParent2, ActorSex.None, pCloneTraits: false, 0, null, pAddToFamily: true).justBorn();
	}

	public static Actor makeBaby(Actor pParent1, Actor pParent2, ActorSex pForcedSexType = ActorSex.None, bool pCloneTraits = false, int pMutationRate = 0, WorldTile pTile = null, bool pAddToFamily = false, bool pJoinFamily = false)
	{
		City tCity = pParent1.city ?? pParent2?.city;
		if (tCity != null)
		{
			tCity.status.housing_free--;
		}
		ActorAsset tActorAsset = pParent1.asset;
		ActorData tNewBabyData = new ActorData();
		tNewBabyData.created_time = World.world.getCurWorldTime();
		tNewBabyData.id = World.world.map_stats.getNextId("unit");
		tNewBabyData.asset_id = tActorAsset.id;
		int tGeneration = pParent1.data.generation;
		if (pParent2 != null && pParent2.data.generation > tGeneration)
		{
			tGeneration = pParent2.data.generation;
		}
		tNewBabyData.generation = tGeneration + 1;
		using ListPool<WorldTile> tListPoolForBabySpawn = new ListPool<WorldTile>(4);
		WorldTile[] neighboursAll = pParent1.current_tile.neighboursAll;
		foreach (WorldTile tTile in neighboursAll)
		{
			if (tTile != pParent1.current_tile && (pParent2 == null || tTile != pParent2.current_tile) && tTile.Type.ground)
			{
				tListPoolForBabySpawn.Add(tTile);
			}
		}
		WorldTile tTargetTile = ((pTile != null) ? pTile : ((tListPoolForBabySpawn.Count != 0) ? tListPoolForBabySpawn.GetRandom() : pParent1.current_tile));
		Actor tNewActorBaby = World.world.units.createBabyActorFromData(tNewBabyData, tTargetTile, tCity);
		tNewActorBaby.setParent1(pParent1);
		if (pParent2 != null)
		{
			tNewActorBaby.setParent2(pParent2);
		}
		if (pAddToFamily && !pParent1.hasFamily())
		{
			World.world.families.newFamily(pParent1, pParent1.current_tile, pParent2);
		}
		else if (pJoinFamily)
		{
			Family tFamily = null;
			tFamily = (pParent1.hasFamily() ? pParent1.family : World.world.families.newFamily(pParent1, pParent1.current_tile, pParent2));
			if (tFamily != null)
			{
				tNewActorBaby.setFamily(tFamily);
			}
		}
		BabyHelper.applyParentsMeta(pParent1, pParent2, tNewActorBaby);
		if (pCloneTraits || pParent1.hasSubspeciesTrait("genetic_mirror"))
		{
			BabyHelper.traitsClone(tNewActorBaby, pParent1);
		}
		else
		{
			foreach (ActorTrait tTrait in tNewActorBaby.subspecies.getActorBirthTraits().getTraits())
			{
				tNewActorBaby.addTrait(tTrait);
			}
			BabyHelper.traitsInherit(tNewActorBaby, pParent1, pParent2);
		}
		tNewActorBaby.checkTraitMutationOnBirth();
		tNewActorBaby.setNutrition(SimGlobals.m.nutrition_start_level_baby);
		if (pForcedSexType != ActorSex.None)
		{
			tNewActorBaby.data.sex = pForcedSexType;
		}
		else
		{
			ActorSex tForcedSexTypeByMeta = ActorSex.None;
			if (Randy.randomBool())
			{
				tForcedSexTypeByMeta = (pParent1.hasCity() ? ((pParent1.city.status.females <= pParent1.city.status.males) ? ActorSex.Female : ActorSex.Male) : ((pParent1.subspecies.cached_females <= pParent1.subspecies.cached_males) ? ActorSex.Female : ActorSex.Male));
			}
			if (tForcedSexTypeByMeta != ActorSex.None)
			{
				tNewActorBaby.data.sex = tForcedSexTypeByMeta;
			}
			else
			{
				tNewActorBaby.generateSex();
			}
		}
		tNewActorBaby.checkShouldBeEgg();
		tNewActorBaby.makeStunned(10f);
		tNewActorBaby.applyRandomForce();
		BabyHelper.countBirth(tNewActorBaby);
		BabyHelper.countMakeChild(pParent1, pParent2);
		tNewActorBaby.setStatsDirty();
		tNewActorBaby.event_full_stats = true;
		return tNewActorBaby;
	}
}
