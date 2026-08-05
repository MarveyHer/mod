using System.Collections.Generic;
using ai;
using UnityEngine;

public static class ActionLibrary
{
	public static bool unluckyMeteorite(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!WorldLawLibrary.world_law_disasters_nature.isEnabled())
		{
			return false;
		}
		if (World.world.cities.Count < 5)
		{
			return false;
		}
		if (pTarget.a.getAge() < 30)
		{
			return false;
		}
		if (!Randy.randomChance(5E-05f))
		{
			return false;
		}
		Meteorite.spawnMeteoriteDisaster(pTarget.current_tile, pTarget.a);
		return true;
	}

	public static bool unluckyFall(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (Randy.randomChance(0.8f))
		{
			return false;
		}
		pTarget.a.makeStunned();
		return true;
	}

	public static bool flamingWeapon(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!MapBox.isRenderGameplay())
		{
			return false;
		}
		if (pTarget.isBuilding())
		{
			return false;
		}
		Actor tActor = pTarget.a;
		if (!tActor.a.is_visible)
		{
			return false;
		}
		Sprite tItemSprite = tActor.getRenderedItemSprite();
		if (tItemSprite == null)
		{
			return false;
		}
		AnimationFrameData tFrameData = tActor.getAnimationFrameData();
		if (tFrameData == null)
		{
			return false;
		}
		Vector3 tVec = new Vector3
		{
			x = tActor.cur_transform_position.x + tFrameData.pos_item.x * tActor.current_scale.x,
			y = tActor.cur_transform_position.y + tFrameData.pos_item.y * tActor.current_scale.y,
			z = -0.01f
		};
		float tWeaponHeight = tItemSprite.rect.height * tActor.current_scale.y;
		if (tActor.is_moving)
		{
			tVec.y += tWeaponHeight;
			tVec.x += Randy.randomFloat(-0.1f, 0.1f);
			tVec.y += Randy.randomFloat(-0.1f, 0.2f);
		}
		else
		{
			tVec.x += Randy.randomFloat(-0.05f, 0.05f);
			float yPlus = Randy.randomFloat(0f, tWeaponHeight * 1.5f);
			if ((double)yPlus < (double)tWeaponHeight * 0.5)
			{
				tVec.x += Randy.randomFloat(-0.15f, 0.15f);
			}
			tVec.y += yPlus;
		}
		if (tActor.current_rotation.y != 0f || tActor.current_rotation.z != 0f)
		{
			tVec = Toolbox.RotatePointAroundPivot(ref tVec, ref tActor.cur_transform_position, ref tActor.current_rotation);
		}
		BaseEffect tEffects = EffectsLibrary.spawn("fx_weapon_particle");
		if (tEffects != null)
		{
			((StatusParticle)tEffects).spawnParticle(tVec, Toolbox.colors_fire.GetRandom());
			return true;
		}
		return false;
	}

	public static bool shiny(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTile.has_tile_up)
		{
			return false;
		}
		if (!MapBox.isRenderGameplay())
		{
			return false;
		}
		Vector3 tVec = pTile.tile_up.posV3;
		tVec.x += Randy.randomFloat(-0.3f, 0.3f);
		tVec.y += Randy.randomFloat(-0.3f, 0.3f);
		EffectsLibrary.spawnAt("fx_building_sparkle", tVec, 0.1f);
		return true;
	}

	public static bool restoreHealthOnHit(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget == null)
		{
			return false;
		}
		if (!pTarget.isActor())
		{
			return false;
		}
		if (!pSelf.isActor())
		{
			return false;
		}
		if (!pSelf.isAlive())
		{
			return false;
		}
		int tHealthToRestore = pTarget.getMaxHealthPercent(0.05f);
		pSelf.a.restoreHealth(tHealthToRestore);
		return true;
	}

	public static void throwTorchAtTile(BaseSimObject pSelf, WorldTile pTile)
	{
		Vector2Int tAttackPosition = pTile.pos;
		Vector3 tSelfPosition = pSelf.current_position;
		float tDist = Vector2.Distance(tSelfPosition, tAttackPosition);
		Vector3 tAttackVector = Toolbox.getNewPoint(tSelfPosition.x, tSelfPosition.y, tAttackPosition.x, tAttackPosition.y, tDist);
		Vector3 tStartProjectile = Toolbox.getNewPoint(tSelfPosition.x, tSelfPosition.y, tAttackPosition.x, tAttackPosition.y, pSelf.a.stats["size"]);
		tStartProjectile.y += 0.5f;
		World.world.projectiles.spawn(pSelf, null, "torch", tStartProjectile, tAttackVector);
	}

	public static bool canThrowBomb(BaseSimObject pTarget, WorldTile pTile)
	{
		float tDist = Toolbox.Dist(pTarget.a.current_position.x, pTarget.a.current_position.y, pTile.pos.x, pTile.pos.y);
		if (tDist > 3f && tDist < 26f)
		{
			return true;
		}
		return false;
	}

	public static void throwBombAtTile(BaseSimObject pSelf, WorldTile pTile)
	{
		Vector2Int tAttackPosition = pTile.pos;
		Vector3 tSelfPosition = pSelf.current_position;
		float tDist = Vector2.Distance(tSelfPosition, tAttackPosition);
		Vector3 tAttackVector = Toolbox.getNewPoint(tSelfPosition.x, tSelfPosition.y, tAttackPosition.x, tAttackPosition.y, tDist);
		Vector3 tStartProjectile = Toolbox.getNewPoint(tSelfPosition.x, tSelfPosition.y, tAttackPosition.x, tAttackPosition.y, pSelf.a.stats["size"]);
		tStartProjectile.y += 0.5f;
		World.world.projectiles.spawn(pSelf, null, "firebomb", tStartProjectile, tAttackVector);
	}

	public static bool zombieInfectAttack(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (!pTarget.isActor())
		{
			return false;
		}
		if (Randy.randomChance(0.25f))
		{
			pTarget.a.startShake(0.2f, 0.05f, pHorizontal: true, pVertical: false);
		}
		pTarget.a.spawnParticle(Toolbox.color_infected);
		if (pTarget.a.asset.can_turn_into_zombie && Randy.randomChance(0.5f))
		{
			pTarget.a.addTrait("infected");
		}
		return true;
	}

	public static bool zombieEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		pTarget.a.spawnParticle(Toolbox.color_infected);
		if (Randy.randomChance(0.25f))
		{
			pTarget.a.startShake(0.2f, 0.05f, pHorizontal: true, pVertical: false);
		}
		return true;
	}

	public static bool infectedEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		int tDamage = pTarget.getHealth() / 10;
		if (tDamage < 10)
		{
			tDamage = 10;
		}
		pTarget.a.getHit(tDamage, pFlash: true, AttackType.Infection, null, pSkipIfShake: false);
		pTarget.a.spawnParticle(Toolbox.color_infected);
		pTarget.a.startShake(0.4f, 0.2f, pHorizontal: true, pVertical: false);
		return true;
	}

	public static bool mushSporesEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		int tMax = 3;
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 3f, pRandom: true))
		{
			if (tActor != pTarget.a && !Randy.randomChance(0.7f) && tActor.addTrait("mush_spores"))
			{
				tActor.spawnParticle(Toolbox.color_mushSpores);
				tMax--;
				if (tMax == 0)
				{
					break;
				}
			}
		}
		return true;
	}

	public static bool tumorEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		pTarget.a.startShake(0.4f, 0.2f, pHorizontal: true, pVertical: false);
		if (Randy.randomChance(0.1f))
		{
			pTarget.getHit(pTarget.getMaxHealthPercent(0.1f), pFlash: false, AttackType.Tumor, null, pSkipIfShake: false);
		}
		return true;
	}

	public static bool healingAuraEffect(BaseSimObject pSelf, WorldTile pTile = null)
	{
		if (!Randy.randomChance(0.2f))
		{
			return false;
		}
		foreach (Actor tActor in Finder.getUnitsFromChunk(pTile, 1, 4f, pRandom: true))
		{
			if (tActor != pSelf.a && !tActor.hasMaxHealth() && !pSelf.areFoes(tActor))
			{
				tActor.restoreHealth(10);
				tActor.spawnParticle(Toolbox.color_heal);
				tActor.removeTrait("plague");
			}
		}
		return true;
	}

	public static bool heliophobiaEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor tActorTarget = pTarget.a;
		BiomeAsset tBiomeAsset = tActorTarget.current_tile.getBiome();
		if (tBiomeAsset != null)
		{
			if (tBiomeAsset.cold_biome)
			{
				return false;
			}
			if (tBiomeAsset.dark_biome)
			{
				return false;
			}
		}
		if (!World.world_era.flag_light_damage)
		{
			return false;
		}
		int tDamage = (int)((float)tActorTarget.getMaxHealth() * 0.1f) + 1;
		tActorTarget.getHit(tDamage, pFlash: true, AttackType.Other, null, pSkipIfShake: true, pMetallicWeapon: false, pCheckDamageReduction: true);
		return true;
	}

	public static bool regenerationEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor tActorTarget = pTarget.a;
		if (tActorTarget.hasTrait("infected"))
		{
			return true;
		}
		if (!tActorTarget.hasMaxHealth() && !tActorTarget.isHungry() && Randy.randomChance(0.2f))
		{
			int tHealthToRegen = tActorTarget.getMaxHealthPercent(0.02f);
			tActorTarget.restoreHealth(tHealthToRegen);
			tActorTarget.spawnParticle(Toolbox.color_heal);
		}
		checkRegenerationTraits(tActorTarget);
		return true;
	}

	private static void checkRegenerationTraits(Actor pActorTarget)
	{
		if (pActorTarget.hasTrait("crippled") && Randy.randomChance(0.05f))
		{
			pActorTarget.removeTrait("crippled");
		}
		if (pActorTarget.hasTrait("skin_burns") && Randy.randomChance(0.05f))
		{
			pActorTarget.removeTrait("skin_burns");
		}
		if (pActorTarget.hasTrait("eyepatch") && Randy.randomChance(0.05f))
		{
			pActorTarget.removeTrait("eyepatch");
		}
	}

	public static bool regenerationEffectClan(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor tActorTarget = pTarget.a;
		if (tActorTarget.hasTrait("infected"))
		{
			return true;
		}
		if (!tActorTarget.hasMaxHealth() && !tActorTarget.isHungry() && Randy.randomChance(0.2f))
		{
			int tHealthToRegen = tActorTarget.getMaxHealthPercent(0.01f);
			tActorTarget.restoreHealth(tHealthToRegen);
			tActorTarget.spawnParticle(Toolbox.color_heal);
		}
		checkRegenerationTraits(tActorTarget);
		return true;
	}

	public static bool suprisedByArchitector(BaseSimObject _, WorldTile pTile)
	{
		if (World.world.isPaused())
		{
			return false;
		}
		foreach (Actor item in Finder.getUnitsFromChunk(pTile, 1, 8f))
		{
			item.tryToGetSurprised(pTile);
		}
		return true;
	}

	public static bool coldAuraEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		World.world.loopWithBrush(pTarget.current_tile, Brush.get(4), PowerLibrary.drawTemperatureMinus);
		return true;
	}

	public static bool megaHeartbeat(BaseSimObject pTarget, WorldTile pTile = null)
	{
		World.world.applyForceOnTile(pTile, 3, 0.3f, pForceOut: true, 0, null, pTarget);
		EffectsLibrary.spawnExplosionWave(pTile.posV3, 3f, 0.5f);
		return true;
	}

	public static bool thornsDefense(BaseSimObject pSelf, BaseSimObject pAttackedBy, WorldTile pTile = null)
	{
		if (pSelf.isAlive() && Randy.randomChance(0.5f))
		{
			if (pAttackedBy != null && pAttackedBy.isActor() && pAttackedBy.isAlive())
			{
				Actor tAttacker = pAttackedBy.a;
				if (Toolbox.DistTile(pSelf.a.current_tile, tAttacker.a.current_tile) < 2f)
				{
					float tDamage = tAttacker.stats["damage"] * 0.2f;
					tAttacker.getHit(tDamage, pFlash: true, AttackType.Weapon, pSelf);
				}
			}
			return true;
		}
		return false;
	}

	public static bool bubbleDefense(BaseSimObject pSelf, BaseSimObject pAttackedBy, WorldTile pTile = null)
	{
		if (pSelf.hasHealth() && Randy.randomChance(0.1f))
		{
			pSelf.addStatusEffect("shield", 5f);
			return true;
		}
		return false;
	}

	public static bool plagueEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		tickPlagueInfection(pTarget.a);
		pTarget.a.startShake(0.4f, 0.2f, pHorizontal: true, pVertical: false);
		if (Randy.randomChance(0.1f))
		{
			int tDamage = pTarget.getMaxHealthPercent(0.15f) + 1;
			pTarget.a.getHit(tDamage, pFlash: false, AttackType.Plague, null, pSkipIfShake: false);
		}
		return true;
	}

	public static bool energizedLightning(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!Toolbox.inMapBorder(ref pTarget.current_position))
		{
			EffectsLibrary.spawnAt("fx_lightning_small", pTarget.current_position, 0.25f);
			return true;
		}
		MapBox.spawnLightningSmall(pTarget.current_tile, 0.25f, pTarget.a);
		return true;
	}

	public static bool contagiousEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!WorldLawLibrary.world_law_rat_plague.isEnabled())
		{
			return false;
		}
		if (Randy.randomChance(0.7f) && ActorTool.countContagiousNearby(pTarget.a) > 20 && Randy.randomChance(0.2f))
		{
			tickPlagueInfection(pTarget.a);
		}
		return true;
	}

	public static bool deathMark(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (Randy.randomChance(0.2f))
		{
			pTarget.a.getHitFullHealth(AttackType.Divine);
		}
		return true;
	}

	private static void tickPlagueInfection(Actor pActor)
	{
		pActor.spawnParticle(Toolbox.color_plague);
		if (!Randy.randomChance(0.05f))
		{
			return;
		}
		int tMax = 3;
		foreach (Actor tActor in Finder.getUnitsFromChunk(pActor.current_tile, 0, 6f, pRandom: true))
		{
			if (tActor != pActor)
			{
				if (tActor.addTrait("plague"))
				{
					break;
				}
				tMax--;
				if (tMax <= 0)
				{
					break;
				}
			}
		}
	}

	public static bool burningFeetEffectTileDraw(WorldTile pTile, string pPowerID)
	{
		if (pTile.isTemporaryFrozen() && Randy.randomBool())
		{
			pTile.unfreeze();
		}
		return true;
	}

	public static bool burningFeetEffect(BaseSimObject pSelf, WorldTile pTile = null)
	{
		WorldTile tCurTile = pSelf.current_tile;
		if (!tCurTile.Type.can_be_set_on_fire_by_burning_feet)
		{
			return false;
		}
		Actor tActor = pSelf.a;
		if (tActor.isInLiquid())
		{
			return false;
		}
		if (!tActor.has_attack_target && !tActor.hasTag("moody"))
		{
			return false;
		}
		World.world.loopWithBrush(tCurTile, Brush.get(4), burningFeetEffectTileDraw);
		tCurTile.startFire(pForce: true);
		for (int i = 0; i < tCurTile.neighbours.Length; i++)
		{
			WorldTile obj = tCurTile.neighbours[i];
			obj.startFire();
			obj.setBurned();
		}
		return true;
	}

	public static bool flowerPrintsEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!Randy.randomChance(0.3f))
		{
			return false;
		}
		WorldTile tTile = pTarget.a.current_tile;
		BiomeAsset tBiomeAsset = tTile.Type.biome_asset;
		if (tBiomeAsset == null)
		{
			return false;
		}
		if (!tBiomeAsset.grow_vegetation_auto)
		{
			return false;
		}
		if (tBiomeAsset.grow_type_selector_plants != null)
		{
			BuildingActions.tryGrowVegetationRandom(tTile, VegetationType.Plants, pOnStart: false, pCheckLimit: false);
		}
		return true;
	}

	public static bool acidBloodEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		for (int i = 0; i < 5; i++)
		{
			if (Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTarget.a.current_tile, "acid", 0f, 0.1f, 5f, 0.5f, 4f, 0.15f);
			}
		}
		if (!pTarget.isActor())
		{
			return true;
		}
		if (pTarget.a.asset.actor_size < ActorSize.S17_Dragon)
		{
			return true;
		}
		for (int j = 0; j < 25; j++)
		{
			if (Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTarget.a.current_tile, "acid", 0f, 0.1f, 10f, 0.5f, 10f, 0.15f);
			}
			for (int k = 0; k < pTarget.a.current_tile.neighboursAll.Length; k++)
			{
				WorldTile tTile = pTarget.a.current_tile.neighboursAll[k];
				if (Randy.randomBool())
				{
					World.world.drop_manager.spawnParabolicDrop(tTile, "acid", 0f, 0.1f, 10f, 0.5f, 7f, 0.15f);
				}
			}
		}
		return true;
	}

	public static bool acidTouchEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!Randy.randomChance(0.3f))
		{
			return false;
		}
		MapAction.checkAcidTerraform(pTarget.a.current_tile);
		return true;
	}

	public static bool sunblessedEffect(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!Randy.randomChance(0.5f))
		{
			return false;
		}
		if (World.world.era_manager.getCurrentAge().flag_night)
		{
			return false;
		}
		float tRandomHealth = Randy.randomFloat(0.05f, 0.1f);
		pTarget.a.restoreHealthPercent(tRandomHealth);
		return true;
	}

	public static bool castSpawnSkeleton(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			pTile = pTarget.current_tile;
		}
		int tCount = 0;
		foreach (Actor item in Finder.findSpeciesAroundTileChunk(pTile, "skeleton"))
		{
			_ = item;
			if (tCount++ > 6)
			{
				return false;
			}
		}
		WorldTile tTile = pTile?.region?.getRandomTile();
		if (tTile == null)
		{
			return false;
		}
		spawnSkeleton(pSelf, tTile);
		return true;
	}

	public static bool spawnSkeleton(BaseSimObject pCaster, WorldTile pTile = null)
	{
		if (pTile == null)
		{
			return false;
		}
		BaseEffect baseEffect = EffectsLibrary.spawnAt("fx_create_skeleton", pTile.posV3, 0.1f);
		Actor tActorCaster = pCaster.a;
		Subspecies tTargetSubspecies = null;
		TileZone current_zone = tActorCaster.current_zone;
		bool tNeedNewSkeletonForm = false;
		Subspecies tSubspeciesTargetForNewSkeleton = null;
		City tCityForSubspecies = current_zone.city;
		if (tCityForSubspecies != null && !tCityForSubspecies.kingdom.isNeutral())
		{
			Subspecies tMainEnemyCitySubspecies = tCityForSubspecies.getMainSubspecies();
			tTargetSubspecies = tMainEnemyCitySubspecies?.getSkeletonForm();
			if (tTargetSubspecies == null)
			{
				tNeedNewSkeletonForm = true;
				tSubspeciesTargetForNewSkeleton = tMainEnemyCitySubspecies;
			}
		}
		else if (tActorCaster.hasCity())
		{
			tCityForSubspecies = tActorCaster.city;
			tTargetSubspecies = tCityForSubspecies.getSubspecies("skeleton");
		}
		baseEffect.setCallback(19, delegate
		{
			Actor actor = World.world.units.createNewUnit("skeleton", pTile, pMiracleSpawn: false, 0f, tTargetSubspecies, null, pSpawnWithItems: true, pAdultAge: true);
			actor.makeWait(1f);
			if (!tActorCaster.isRekt())
			{
				if (actor.subspecies.isJustCreated() && tActorCaster.isKingdomCiv())
				{
					actor.subspecies.addTrait("prefrontal_cortex");
				}
				if (actor.subspecies.isJustCreated() && tNeedNewSkeletonForm && !tSubspeciesTargetForNewSkeleton.isRekt())
				{
					tSubspeciesTargetForNewSkeleton.setSkeletonForm(actor.subspecies);
				}
				if (tActorCaster.isKingdomCiv() && actor.subspecies.hasTrait("prefrontal_cortex"))
				{
					City city = tActorCaster.city;
					Kingdom kingdom = tActorCaster.kingdom;
					if (!city.isRekt() && city.kingdom == kingdom)
					{
						actor.joinCity(tActorCaster.city);
					}
					else
					{
						actor.joinKingdom(kingdom);
					}
				}
			}
		});
		return true;
	}

	public static bool castFire(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "fire", 15f, -1f, -1L);
		for (int i = 0; i < 3; i++)
		{
			World.world.drop_manager.spawn(pTile.neighboursAll.GetRandom(), "fire", 15f, -1f, -1L);
		}
		return true;
	}

	public static bool castSpellSilence(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "spell_silence", 15f, -1f, -1L);
		return true;
	}

	public static bool castBloodRain(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "blood_rain", 15f, -1f, pSelf.id);
		return true;
	}

	public static bool castSpawnGrassSeeds(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTile == null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		if (WorldLawLibrary.world_law_gaias_covenant.isEnabled())
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "seeds_grass", 15f, -1f, -1L);
		return true;
	}

	public static bool castSpawnFertilizer(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTile == null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "fertilizer_trees", 15f, -1f, -1L);
		return true;
	}

	public static bool castCurses(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			if (pTarget.a.hasStatus("cursed"))
			{
				return false;
			}
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "curse", 15f, -1f, -1L);
		return true;
	}

	public static bool castLightning(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			pTile = pTarget.current_tile;
		}
		MapBox.spawnLightningMedium(pTile, 0.15f, pSelf.a);
		return true;
	}

	public static bool castTornado(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget != null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		(EffectsLibrary.spawnAtTile("fx_tornado", pTile, 1f / 12f) as TornadoEffect).resizeTornado(1f / 6f);
		return true;
	}

	public static bool castCure(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTile == null)
		{
			pTile = pTarget.current_tile;
		}
		if (pTile == null)
		{
			return false;
		}
		World.world.drop_manager.spawn(pTile, "cure", 15f, -1f, -1L);
		return true;
	}

	public static bool castShieldOnHimself(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		return addShieldEffectOnTarget(pSelf, pTarget);
	}

	public static bool addShieldEffectOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget.hasStatus("shield"))
		{
			return false;
		}
		pTarget.a.addStatusEffect("shield", 30f);
		return true;
	}

	public static bool addBurningEffectOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.isBuilding() && pTarget.b.isBurnable())
		{
			pTarget.addStatusEffect("burning");
			return true;
		}
		if (pTarget.isActor())
		{
			pTarget.addStatusEffect("burning");
			return true;
		}
		return false;
	}

	public static bool addFrozenEffectOnTarget20(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.isBuilding())
		{
			return false;
		}
		if (Randy.randomChance(0.2f))
		{
			return addFrozenEffectOnTarget(pSelf, pTarget, pTile);
		}
		return false;
	}

	public static bool addStunnedEffectOnTarget20(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget.isRekt())
		{
			return false;
		}
		if (pTarget.isBuilding())
		{
			return false;
		}
		if (Randy.randomChance(0.2f))
		{
			return addStunnedEffectOnTarget(pSelf, pTarget, pTile);
		}
		return false;
	}

	public static bool addStunnedEffectOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget.isRekt())
		{
			return false;
		}
		if (pTarget.isBuilding())
		{
			return false;
		}
		pTarget.addStatusEffect("stunned");
		return true;
	}

	public static bool addFrozenEffectOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget.isBuilding())
		{
			return false;
		}
		if (pTarget.current_tile.Type.lava)
		{
			return false;
		}
		if (pTarget.current_tile.isOnFire())
		{
			return false;
		}
		pTarget.addStatusEffect("frozen");
		return true;
	}

	public static bool addSlowEffectOnTarget20(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.isBuilding())
		{
			return false;
		}
		if (Randy.randomChance(0.2f))
		{
			return addSlowEffectOnTarget(pSelf, pTarget, pTile);
		}
		return false;
	}

	public static bool addSlowEffectOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget.isBuilding())
		{
			return false;
		}
		pTarget.addStatusEffect("slowness");
		return true;
	}

	public static bool addPoisonedEffectOnTarget(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTarget.isActor())
		{
			return false;
		}
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.a.hasTrait("poison_immune"))
		{
			return false;
		}
		if (!pTarget.a.asset.has_skin)
		{
			return false;
		}
		if (pTarget.a.asset.immune_to_injuries)
		{
			return false;
		}
		if (Randy.randomChance(0.3f))
		{
			pTarget.a.addStatusEffect("poisoned");
		}
		return false;
	}

	public static void increaseDroppedBombsCounter(WorldTile pTile = null, string pDropID = null)
	{
		World.world.game_stats.data.bombsDropped++;
		AchievementLibrary.many_bombs.check();
	}

	public static bool giveCursed(WorldTile pTile, Actor pActor)
	{
		if (pActor.hasSubspecies() && pActor.subspecies.hasTrait("adaptation_corruption"))
		{
			return false;
		}
		bool num = pActor.addStatusEffect("cursed");
		if (num)
		{
			pActor.removeTrait("blessed");
		}
		return num;
	}

	public static bool singularityTeleportation(WorldTile pTile, Actor pActor)
	{
		BiomeAsset tBiome = AssetManager.biome_library.get("biome_singularity");
		WorldTile tTileTarget = null;
		if (tBiome.getTileHigh().hashset.Count > 0 && Randy.randomBool())
		{
			tTileTarget = tBiome.getTileHigh().hashset.GetRandom();
		}
		else if (tBiome.getTileLow().hashset.Count > 0)
		{
			tTileTarget = tBiome.getTileLow().hashset.GetRandom();
		}
		if (tTileTarget == null)
		{
			return false;
		}
		EffectsLibrary.spawnAt("fx_teleport_singularity", tTileTarget.posV3, pActor.stats["scale"] * 1.2f);
		EffectsLibrary.spawnAt("fx_teleport_singularity", pActor.current_position, pActor.stats["scale"] * 1.2f);
		pActor.cancelAllBeh();
		pActor.spawnOn(tTileTarget);
		pActor.makeStunned();
		return true;
	}

	public static bool timeParadox(WorldTile pTile, Actor pActor)
	{
		if (pActor.isAlive())
		{
			pActor.data.age_overgrowth++;
			return true;
		}
		return false;
	}

	public static bool giveEnchanted(WorldTile pTile, Actor pActor)
	{
		pActor.finishStatusEffect("cursed");
		return pActor.addStatusEffect("enchanted");
	}

	public static bool spawnGhost(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTarget.isActor())
		{
			return false;
		}
		if (!pTarget.a.asset.has_soul)
		{
			return false;
		}
		Actor tGhost = World.world.units.createNewUnit("ghost", pTile);
		tGhost.removeTrait("blessed");
		ActorTool.copyUnitToOtherUnit(pTarget.a, tGhost);
		return true;
	}

	public static bool tryToGrowBiomeGrass(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (!pTile.Type.can_be_biome)
		{
			return false;
		}
		if (pTile.Type.is_biome)
		{
			return false;
		}
		DropsLibrary.useSeedOn(pTile, TopTileLibrary.grass_low, TopTileLibrary.grass_high);
		return true;
	}

	public static bool tryToGrowTree(BaseSimObject pTarget, WorldTile pTile = null)
	{
		BuildingActions.tryGrowVegetationRandom(pTile, VegetationType.Trees, pOnStart: false, pCheckLimit: false);
		return true;
	}

	public static bool tryToCreatePlants(BaseSimObject pTarget, WorldTile pTile = null)
	{
		BiomeAsset tBiomeAsset = pTarget.current_tile.Type.biome_asset;
		if (tBiomeAsset == null)
		{
			return false;
		}
		if (tBiomeAsset.grow_type_selector_plants != null)
		{
			BuildingActions.tryGrowVegetationRandom(pTarget.current_tile, VegetationType.Plants);
		}
		return true;
	}

	public static bool startNuke(BaseSimObject pTarget, WorldTile pTile = null)
	{
		pTarget.a.findCurrentTile();
		EffectsLibrary.spawn("fx_nuke_flash", pTile, "atomic_bomb");
		return true;
	}

	public static bool clearCrabzilla(BaseSimObject pTarget, WorldTile pTile = null)
	{
		MusicBox.inst.stopDrawingSound("event:/SFX/UNIQUE/Crabzilla/CrabzillaLazer");
		MusicBox.inst.stopDrawingSound("event:/SFX/UNIQUE/Crabzilla/CrabzillaVoice");
		if (Config.joyControls)
		{
			UltimateJoystick.ResetJoysticks();
		}
		return true;
	}

	public static bool startCrabzillaNuke(BaseSimObject pTarget, WorldTile pTile = null)
	{
		pTarget.a.findCurrentTile();
		EffectsLibrary.spawn("fx_nuke_flash", pTile, "crabzilla_bomb");
		return true;
	}

	public static bool deathNuke(BaseSimObject pTarget, WorldTile pTile = null)
	{
		pTarget.a.findCurrentTile();
		DropsLibrary.action_atomic_bomb(pTarget.current_tile);
		return true;
	}

	public static bool deathBomb(BaseSimObject pTarget, WorldTile pTile = null)
	{
		pTarget.a.findCurrentTile();
		DropsLibrary.action_bomb(pTarget.current_tile);
		return true;
	}

	public static bool spawnAliens(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor a = pTarget.a;
		a.findCurrentTile();
		if (!a.inMapBorder())
		{
			return false;
		}
		int tAmount = 1;
		if (Randy.randomChance(0.5f))
		{
			tAmount++;
		}
		if (Randy.randomChance(0.1f))
		{
			tAmount++;
		}
		for (int i = 0; i < tAmount; i++)
		{
			World.world.units.createNewUnit("alien", pTarget.a.current_tile, pMiracleSpawn: false, pTarget.a.position_height, null, null, pSpawnWithItems: true, pAdultAge: true);
		}
		return true;
	}

	public static bool fireDropsSpawn(BaseSimObject pTarget, WorldTile pTile = null)
	{
		for (int i = 0; i < 5; i++)
		{
			if (Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTarget.a.current_tile, "fire", 0f, 0.1f, 5f, 0.5f, 4f, 0.15f);
			}
		}
		if (!pTarget.isActor())
		{
			return true;
		}
		if (pTarget.a.asset.actor_size < ActorSize.S17_Dragon)
		{
			return true;
		}
		for (int j = 0; j < 25; j++)
		{
			if (Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTarget.a.current_tile, "fire", 0f, 0.1f, 10f, 0.5f, 10f, 0.15f);
			}
			for (int k = 0; k < pTarget.a.current_tile.neighboursAll.Length; k++)
			{
				WorldTile tTile = pTarget.a.current_tile.neighboursAll[k];
				if (Randy.randomBool())
				{
					World.world.drop_manager.spawnParabolicDrop(tTile, "fire", 0f, 0.1f, 10f, 0.5f, 7f, 0.15f);
				}
			}
		}
		return true;
	}

	public static bool snowDropsSpawn(BaseSimObject pTarget, WorldTile pTile = null)
	{
		for (int i = 0; i < 20; i++)
		{
			if (Randy.randomBool())
			{
				World.world.drop_manager.spawnParabolicDrop(pTarget.a.current_tile, "snow", 0f, 0.1f, 5f, 0.5f, 4f, 0.15f);
			}
		}
		return true;
	}

	public static bool teleportRandom(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile = null)
	{
		WorldTile tTile = World.world.islands_calculator.getRandomIslandGround()?.regions.GetRandom()?.tiles.GetRandom();
		if (tTile == null)
		{
			return false;
		}
		if (tTile.Type.block)
		{
			return false;
		}
		if (!tTile.Type.ground)
		{
			return false;
		}
		teleportEffect(pTarget.a, tTile);
		pTarget.a.cancelAllBeh();
		pTarget.a.spawnOn(tTile);
		return true;
	}

	public static void teleportEffect(Actor pActor, WorldTile pTile)
	{
		string tTeleportEffect = pActor.asset.effect_teleport;
		if (string.IsNullOrEmpty(tTeleportEffect))
		{
			tTeleportEffect = "fx_teleport_blue";
		}
		EffectsLibrary.spawnAt(tTeleportEffect, pActor.current_position, pActor.stats["scale"]);
		BaseEffect tEffect = EffectsLibrary.spawnAt(tTeleportEffect, pTile.posV3, pActor.stats["scale"]);
		if (tEffect != null)
		{
			tEffect.sprite_animation.setFrameIndex(9);
		}
	}

	public static bool metamorphInto(Actor pTarget, string pAsset, bool pRemoveAcquiredTraits = false, bool pUseCurrentSubspecies = false)
	{
		if (pTarget == null)
		{
			return false;
		}
		if (!pTarget.inMapBorder())
		{
			return false;
		}
		if (pTarget.isAlreadyTransformed())
		{
			return false;
		}
		pTarget.finishStatusEffect("cursed");
		pTarget.removeTrait("infected");
		pTarget.removeTrait("mush_spores");
		pTarget.removeTrait("tumor_infection");
		if (pRemoveAcquiredTraits)
		{
			IReadOnlyCollection<ActorTrait> tTraits = pTarget.getTraits();
			using ListPool<ActorTrait> tToRemove = new ListPool<ActorTrait>(tTraits.Count);
			foreach (ActorTrait tTrait in tTraits)
			{
				if (tTrait.group_id == "acquired")
				{
					tToRemove.Add(tTrait);
				}
			}
			pTarget.removeTraits(tToRemove);
		}
		Subspecies tPreviousSubspecies = null;
		if (pUseCurrentSubspecies)
		{
			tPreviousSubspecies = pTarget.subspecies;
		}
		Actor tNewUnit = World.world.units.createNewUnit(pAsset, pTarget.current_tile, pMiracleSpawn: false, 0f, tPreviousSubspecies, null, pSpawnWithItems: false);
		ActorTool.copyUnitToOtherUnit(pTarget, tNewUnit, pCopyAge: false);
		EffectsLibrary.spawn("fx_spawn", tNewUnit.current_tile);
		removeUnit(pTarget);
		pTarget.setTransformed();
		tNewUnit.addTrait("metamorphed");
		return true;
	}

	public static bool turnIntoMush(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor pActor = pTarget.a;
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.hasTrait("mush_spores"))
		{
			return false;
		}
		if (!pActor.inMapBorder())
		{
			return false;
		}
		if (!pActor.asset.can_turn_into_mush)
		{
			return false;
		}
		if (pActor.isAlreadyTransformed())
		{
			return false;
		}
		pActor.finishStatusEffect("cursed");
		pActor.removeTrait("infected");
		pActor.removeTrait("mush_spores");
		pActor.removeTrait("tumor_infection");
		pActor.removeTrait("peaceful");
		Actor tMush = World.world.units.createNewUnit(pActor.asset.mush_id, pActor.current_tile, pMiracleSpawn: false, 0f, null, null, pSpawnWithItems: false);
		ActorTool.copyUnitToOtherUnit(pActor, tMush);
		if (MapBox.isRenderGameplay())
		{
			EffectsLibrary.spawn("fx_spawn", tMush.current_tile);
		}
		removeUnit(pTarget.a);
		pActor.setTransformed();
		return true;
	}

	public static Actor turnIntoMetamorph(BaseSimObject pTarget, string pAssetID)
	{
		Actor pActor = pTarget.a;
		if (pActor == null)
		{
			return null;
		}
		if (!pActor.inMapBorder())
		{
			return null;
		}
		if (pActor.isAlreadyTransformed())
		{
			return null;
		}
		pActor.finishStatusEffect("cursed");
		pActor.removeTrait("infected");
		pActor.removeTrait("mush_spores");
		pActor.removeTrait("tumor_infection");
		pActor.removeTrait("peaceful");
		Actor tNewMetamorphedUnit = World.world.units.createNewUnit(pAssetID, pActor.current_tile, pMiracleSpawn: false, 0f, null, null, pSpawnWithItems: false);
		ActorTool.copyUnitToOtherUnit(pActor, tNewMetamorphedUnit);
		EffectsLibrary.spawn("fx_spawn", tNewMetamorphedUnit.current_tile);
		removeUnit(pTarget.a);
		pActor.setTransformed();
		return tNewMetamorphedUnit;
	}

	public static Actor turnIntoIceOne(BaseSimObject pTarget, WorldTile pTile = null)
	{
		return turnIntoMetamorph(pTarget, "cold_one");
	}

	public static bool turnIntoDemon(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor pActor = pTarget.a;
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.inMapBorder())
		{
			return false;
		}
		if (pActor.isAlreadyTransformed())
		{
			return false;
		}
		pActor.finishStatusEffect("cursed");
		pActor.removeTrait("infected");
		pActor.removeTrait("mush_spores");
		pActor.removeTrait("tumor_infection");
		pActor.removeTrait("peaceful");
		Actor tDemon = World.world.units.createNewUnit("demon", pActor.current_tile, pMiracleSpawn: false, 0f, null, null, pSpawnWithItems: false);
		tDemon.addTrait("metamorphed");
		ActorTool.copyUnitToOtherUnit(pActor, tDemon);
		EffectsLibrary.spawn("fx_spawn", tDemon.current_tile);
		removeUnit(pTarget.a);
		pActor.setTransformed();
		return true;
	}

	public static bool turnIntoTumorMonster(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor pActor = pTarget.a;
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.hasTrait("tumor_infection"))
		{
			return false;
		}
		if (!pActor.inMapBorder())
		{
			return false;
		}
		if (!pActor.asset.can_turn_into_tumor)
		{
			return false;
		}
		if (pActor.isAlreadyTransformed())
		{
			return false;
		}
		pActor.finishStatusEffect("cursed");
		pActor.removeTrait("infected");
		pActor.removeTrait("mush_spores");
		pActor.removeTrait("tumor_infection");
		pActor.removeTrait("peaceful");
		Actor tTumorMonster = World.world.units.createNewUnit(pActor.asset.tumor_id, pActor.current_tile, pMiracleSpawn: false, 0f, null, null, pSpawnWithItems: false);
		ActorTool.copyUnitToOtherUnit(pActor, tTumorMonster);
		EffectsLibrary.spawn("fx_spawn", tTumorMonster.current_tile);
		removeUnit(pTarget.a);
		pActor.setTransformed();
		return true;
	}

	public static bool turnIntoZombie(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor pActor = pTarget.a;
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.hasTrait("infected"))
		{
			return false;
		}
		if (!pActor.inMapBorder())
		{
			return false;
		}
		if (!pActor.asset.can_turn_into_zombie)
		{
			return false;
		}
		if (pActor.isAlreadyTransformed())
		{
			return false;
		}
		pActor.finishStatusEffect("cursed");
		pActor.removeTrait("infected");
		pActor.removeTrait("mush_spores");
		pActor.removeTrait("tumor_infection");
		string tStatsID = pActor.asset.getZombieID();
		if (pActor.asset.id == "dragon")
		{
			pActor.removeTrait("fire_blood");
			pActor.removeTrait("fire_proof");
		}
		Actor tZombie = World.world.units.createNewUnit(tStatsID, pActor.current_tile, pMiracleSpawn: false, 0f, null, pActor.subspecies, pSpawnWithItems: false);
		ActorTool.copyUnitToOtherUnit(pActor, tZombie);
		tZombie.removeTrait("fast");
		tZombie.removeTrait("agile");
		tZombie.removeTrait("genius");
		tZombie.removeTrait("peaceful");
		if (!pActor.getName().StartsWith("Un"))
		{
			tZombie.setName("Un" + Toolbox.LowerCaseFirst(pActor.getName()));
		}
		EffectsLibrary.spawn("fx_spawn", tZombie.current_tile);
		removeUnit(pTarget.a);
		pActor.setTransformed();
		return true;
	}

	public static bool turnIntoSkeleton(BaseSimObject pTarget, WorldTile pTile = null)
	{
		Actor pActor = pTarget.a;
		if (string.IsNullOrEmpty(pActor.asset.skeleton_id))
		{
			return false;
		}
		if (pActor == null)
		{
			return false;
		}
		if (!pActor.hasStatus("cursed"))
		{
			return false;
		}
		if (!pActor.inMapBorder())
		{
			return false;
		}
		if (pActor.isAlreadyTransformed())
		{
			return false;
		}
		string tStatsID = pActor.asset.skeleton_id;
		pActor.finishStatusEffect("cursed");
		pActor.removeTrait("infected");
		pActor.removeTrait("mush_spores");
		pActor.removeTrait("tumor_infection");
		Subspecies tTargetSubspecies = null;
		if (pActor.hasSubspecies())
		{
			tTargetSubspecies = pActor.subspecies.getSkeletonForm();
		}
		Actor tSkeletonActor = World.world.units.createNewUnit(tStatsID, pActor.current_tile, pMiracleSpawn: false, 0f, tTargetSubspecies, null, pSpawnWithItems: false);
		Subspecies tSkeletonSubspecies = tSkeletonActor.subspecies;
		if (tSkeletonSubspecies.isJustCreated())
		{
			tTargetSubspecies?.setSkeletonForm(tSkeletonSubspecies);
		}
		ActorTool.copyUnitToOtherUnit(pActor, tSkeletonActor);
		if (!pActor.getName().StartsWith("Un"))
		{
			tSkeletonActor.setName("Un" + Toolbox.LowerCaseFirst(pActor.getName()));
		}
		EffectsLibrary.spawn("fx_spawn", tSkeletonActor.current_tile);
		removeUnit(pTarget.a);
		pActor.setTransformed();
		return true;
	}

	public static Actor getActorNearPos(Vector2 pPos)
	{
		Actor tResult = null;
		float tDistBest = float.MaxValue;
		Actor[] tArr = World.world.units.visible_units.array;
		int tLen = World.world.units.visible_units.count;
		for (int i = 0; i < tLen; i++)
		{
			Actor tActor = tArr[i];
			if (tActor.isAlive() && tActor.asset.can_be_inspected && !tActor.isInsideSomething())
			{
				float tDist = Toolbox.DistVec2Float(tActor.current_position, pPos);
				if (!(tDist > 3f) && tDist < tDistBest)
				{
					tResult = tActor;
					tDistBest = tDist;
				}
			}
		}
		return tResult;
	}

	public static Actor getActorFromTile(WorldTile pTile = null)
	{
		if (pTile == null)
		{
			return null;
		}
		Actor tResult = null;
		float tDistBest = float.MaxValue;
		List<Actor> tActorList = World.world.units.getSimpleList();
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tActor = tActorList[i];
			if (tActor.isAlive())
			{
				float tDist = Toolbox.SquaredDistTile(tActor.current_tile, pTile);
				if (!(tDist > tDistBest) && !(tDist > 9f) && tActor.asset.can_be_inspected && !tActor.isInsideSomething())
				{
					tResult = tActor;
					tDistBest = tDist;
				}
			}
		}
		return tResult;
	}

	public static void openUnitWindow(Actor pActor)
	{
		if (!pActor.isRekt())
		{
			SelectedUnit.clear();
			SelectedUnit.select(pActor);
		}
		else if (!SelectedUnit.isSet())
		{
			return;
		}
		ScrollWindow.showWindow("unit");
	}

	public static bool inspectUnit(WorldTile pTile = null, string pPower = null)
	{
		Actor tResult = null;
		tResult = ((pTile != null) ? getActorFromTile(pTile) : World.world.getActorNearCursor());
		if (tResult == null)
		{
			return false;
		}
		openUnitWindow(tResult);
		return true;
	}

	public static bool inspectUnitSelectedMeta(WorldTile pTile = null, string pPower = null)
	{
		Actor tActor = null;
		tActor = ((pTile != null) ? getActorFromTile(pTile) : World.world.getActorNearCursor());
		if (tActor == null)
		{
			return false;
		}
		MetaTypeAsset tMetaAsset = Zones.getCurrentMapBorderMode().getAsset();
		if (tMetaAsset == null)
		{
			return false;
		}
		if (tMetaAsset.check_unit_has_meta(tActor))
		{
			tMetaAsset.set_unit_set_meta_for_meta_for_window(tActor);
			ScrollWindow.showWindow(tMetaAsset.window_name);
			return true;
		}
		openUnitWindow(tActor);
		return true;
	}

	public static bool inspectCity(WorldTile pTile = null, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.city);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.City.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectKingdom(WorldTile pTile = null, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.kingdom);
		if (tMetaObject == null)
		{
			return false;
		}
		if (((Kingdom)tMetaObject).isNeutral())
		{
			return false;
		}
		MetaType.Kingdom.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectAlliance(WorldTile pTile = null, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		City tCity = pTile.zone.city;
		if (tCity.isRekt())
		{
			return false;
		}
		Kingdom tKingdom = tCity.kingdom;
		if (tKingdom.isRekt())
		{
			return false;
		}
		if (tKingdom.isNeutral())
		{
			return false;
		}
		if (tKingdom.hasAlliance())
		{
			MetaType.Alliance.getAsset().selectAndInspect(tKingdom.getAlliance());
		}
		else
		{
			inspectKingdom(pTile, pPower);
		}
		return true;
	}

	public static bool inspectCulture(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.culture);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Culture.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectReligion(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.religion);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Religion.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectSubspecies(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.subspecies);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Subspecies.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectFamily(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.family);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Family.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectArmy(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.army);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Army.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectLanguage(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.language);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Language.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool inspectClan(WorldTile pTile, string pPower = null)
	{
		if (pTile == null)
		{
			return false;
		}
		NanoObject tMetaObject = getNanoObjectFromTile(pTile, MetaTypeLibrary.clan);
		if (tMetaObject == null)
		{
			return false;
		}
		MetaType.Clan.getAsset().selectAndInspect(tMetaObject);
		return true;
	}

	public static bool burnTile(BaseSimObject pSelf, BaseSimObject pTarget = null, WorldTile pTile = null)
	{
		if (!World.world.flash_effects.contains(pTile) && Randy.randomChance(0.2f))
		{
			World.world.particles_fire.spawn(pTile.posV3);
		}
		pTile.startFire(pForce: true);
		return true;
	}

	public static bool tryToEvolveUnitViaMonolith(Actor pActor)
	{
		pActor.startShake();
		pActor.startColorEffect();
		if (!pActor.hasSubspecies())
		{
			return false;
		}
		if (pActor.hasSubspeciesTrait("pure"))
		{
			return false;
		}
		float tChance = 1f;
		if (pActor.asset.can_evolve_into_new_species)
		{
			tChance = 1f;
		}
		else if (pActor.hasSubspeciesTrait("uplifted") && pActor.subspecies.isSapient())
		{
			tChance = 0.1f;
		}
		if (!Randy.randomChance(tChance))
		{
			return false;
		}
		World.world.units.evolutionEvent(pActor, pWithBiomeEffect: true, pAscension: false);
		return true;
	}

	public static bool tryToEvolveUnitViaAscension(Actor pActor, out Actor pEvolvedActorForm)
	{
		pEvolvedActorForm = null;
		pActor.startShake();
		pActor.startColorEffect();
		if (!pActor.hasSubspecies())
		{
			return false;
		}
		if (pActor.hasSubspeciesTrait("pure"))
		{
			return false;
		}
		Actor tNewActor = World.world.units.evolutionEvent(pActor, pWithBiomeEffect: true, pAscension: true);
		pEvolvedActorForm = tNewActor;
		return true;
	}

	public static void startBurningObjects(BaseSimObject pSelf, BaseSimObject pTarget = null, WorldTile pTile = null)
	{
		List<BaseSimObject> tList = Finder.getAllObjectsInChunks(pTile);
		for (int i = 0; i < tList.Count; i++)
		{
			BaseSimObject tSimObject = tList[i];
			if (tSimObject.isAlive() && !tSimObject.current_tile.Type.ocean)
			{
				addBurningEffectOnTarget(pSelf, tSimObject);
			}
		}
	}

	public static void action_growTornadoes(WorldTile pTile = null, string pDropID = null)
	{
		TornadoEffect.growTornados(pTile);
	}

	public static void action_shrinkTornadoes(WorldTile pTile = null, string pDropID = null)
	{
		TornadoEffect.shrinkTornados(pTile);
	}

	public static bool dragonSlayer(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget == null)
		{
			return false;
		}
		if (!pTarget.isActor())
		{
			return false;
		}
		BaseSimObject tAttacker = pTarget.a.attackedBy;
		if (tAttacker != null && tAttacker.isActor() && tAttacker.isAlive())
		{
			tAttacker.a.addTrait("dragonslayer");
			return true;
		}
		return false;
	}

	public static bool mageSlayerCheck(BaseSimObject pTarget, WorldTile pTile = null)
	{
		if (pTarget == null)
		{
			return false;
		}
		if (!pTarget.isActor())
		{
			return false;
		}
		if (!pTarget.a.hasSpells())
		{
			return false;
		}
		BaseSimObject tAttacker = pTarget.a.attackedBy;
		if (tAttacker != null && tAttacker.isActor() && tAttacker.isAlive())
		{
			tAttacker.a.addTrait("mageslayer");
			return true;
		}
		return false;
	}

	public static bool checkPiranhaAchievement(BaseSimObject pTarget, WorldTile pTile = null)
	{
		AchievementLibrary.piranha_land.check(pTarget.a);
		return true;
	}

	public static bool clickRelations(WorldTile pTile, string pPowerID)
	{
		City tCity = pTile.zone.city;
		if (tCity.isRekt())
		{
			return false;
		}
		Kingdom tKingdom = tCity.kingdom;
		if (tKingdom.isRekt())
		{
			return false;
		}
		if (tKingdom.isNeutral())
		{
			return false;
		}
		if (SelectedMetas.selected_kingdom != tKingdom)
		{
			SelectedMetas.selected_kingdom = tKingdom;
		}
		else
		{
			ScrollWindow.showWindow("kingdom");
		}
		return true;
	}

	public static bool clickWhisperOfWar(WorldTile pTile, string pPowerID)
	{
		City tCity = pTile.zone.city;
		if (tCity.isRekt())
		{
			return false;
		}
		Kingdom tKingdom = tCity.kingdom;
		if (tKingdom.isRekt())
		{
			return false;
		}
		if (tKingdom.isNeutral())
		{
			return false;
		}
		if (Config.whisper_A == null)
		{
			Config.whisper_A = tKingdom;
			showWhisperTip("whisper_selected_first");
			return false;
		}
		if (Config.whisper_B == null && Config.whisper_A == tKingdom)
		{
			showWhisperTip("whisper_cancelled");
			Config.whisper_A = null;
			Config.whisper_B = null;
			return false;
		}
		if (Config.whisper_B == null)
		{
			Config.whisper_B = tKingdom;
		}
		if (Config.whisper_B != Config.whisper_A)
		{
			if (Config.whisper_A.isEnemy(Config.whisper_B))
			{
				showWhisperTip("whisper_already_in_war");
				Config.whisper_B = null;
				return false;
			}
			if (Config.whisper_A.isInWarOnSameSide(Config.whisper_B))
			{
				using ListPool<War> tWars = new ListPool<War>(Config.whisper_A.getWars());
				foreach (ref War item in tWars)
				{
					War tWar = item;
					if (!tWar.isTotalWar() && tWar.onTheSameSide(Config.whisper_A, Config.whisper_B))
					{
						tWar.leaveWar(Config.whisper_B);
					}
				}
			}
			bool tHaveCommonEnemy = World.world.wars.haveCommonEnemy(Config.whisper_A, Config.whisper_B);
			Alliance tAllianceA = Config.whisper_A.getAlliance();
			if (tAllianceA != null && Alliance.isSame(tAllianceA, Config.whisper_B.getAlliance()))
			{
				tAllianceA.leave(Config.whisper_A);
			}
			War tOngoingWar = World.world.wars.getRandomWarFor(Config.whisper_B);
			if (tOngoingWar != null && !tOngoingWar.isTotalWar() && !tHaveCommonEnemy)
			{
				if (tOngoingWar.isAttacker(Config.whisper_B))
				{
					tOngoingWar.joinDefenders(Config.whisper_A);
				}
				else
				{
					tOngoingWar.joinAttackers(Config.whisper_A);
				}
				showWhisperTip("whisper_joined_war");
			}
			else
			{
				World.world.diplomacy.startWar(Config.whisper_A, Config.whisper_B, WarTypeLibrary.whisper_of_war);
				showWhisperTip("whisper_new_war");
			}
			Config.whisper_A.affectKingByPowers();
			Config.whisper_A = null;
			Config.whisper_B = null;
		}
		return true;
	}

	public static bool clickUnity(WorldTile pTile, string pPowerID)
	{
		City tCity = pTile.zone.city;
		if (tCity.isRekt())
		{
			return false;
		}
		Kingdom tKingdom = tCity.kingdom;
		if (tKingdom.isRekt())
		{
			return false;
		}
		if (tKingdom.isNeutral())
		{
			return false;
		}
		if (Config.unity_A == null)
		{
			Config.unity_A = tKingdom;
			showWhisperTip("unity_selected_first");
			return false;
		}
		if (Config.whisper_B == null && Config.unity_A == tKingdom)
		{
			showWhisperTip("unity_cancelled");
			Config.unity_A = null;
			Config.unity_B = null;
			return false;
		}
		if (Config.unity_A.hasAlliance() && tKingdom.hasAlliance() && Config.unity_A.getAlliance() == tKingdom.getAlliance())
		{
			showWhisperTip("unity_cancelled");
			Config.unity_A = null;
			Config.unity_B = null;
			return false;
		}
		if (Config.unity_B == null)
		{
			Config.unity_B = tKingdom;
		}
		if (Config.unity_B == Config.unity_A)
		{
			return false;
		}
		if (Config.unity_A.isEnemy(Config.unity_B))
		{
			showWhisperTip("unity_in_war");
			Config.unity_B = null;
			return false;
		}
		if (Config.unity_A.hasAlliance())
		{
			if (Config.unity_A.getAlliance() == Config.unity_B.getAlliance())
			{
				showWhisperTip("unity_cancelled");
				Config.unity_B = null;
				return false;
			}
			if (Config.unity_B.hasAlliance())
			{
				Config.unity_A.getAlliance().leave(Config.unity_A);
			}
		}
		if (World.world.alliances.forceAlliance(Config.unity_A, Config.unity_B))
		{
			showWhisperTip("unity_new_alliance");
		}
		else
		{
			showWhisperTip("unity_joined_alliance");
		}
		Config.unity_A.affectKingByPowers();
		Config.unity_A = null;
		Config.unity_B = null;
		World.world.zone_calculator.dirtyAndClear();
		return true;
	}

	private static void showWhisperTip(string pText)
	{
		string tLocalizedText = LocalizedTextManager.getText(pText);
		if (Config.whisper_A != null)
		{
			tLocalizedText = tLocalizedText.Replace("$kingdom_A$", Config.whisper_A.name);
		}
		if (Config.whisper_B != null)
		{
			tLocalizedText = tLocalizedText.Replace("$kingdom_B$", Config.whisper_B.name);
		}
		WorldTip.showNow(tLocalizedText, pTranslate: false, "top", 6f);
	}

	public static bool selectWhisperOfWar(string pPowerID)
	{
		WorldTip.showNow("whisper_selected", pTranslate: true, "top");
		Config.whisper_A = null;
		Config.whisper_B = null;
		return false;
	}

	public static bool selectUnity(string pPowerID)
	{
		WorldTip.showNow("unity_selected", pTranslate: true, "top");
		Config.unity_A = null;
		Config.unity_B = null;
		return false;
	}

	public static bool selectRelations(string pPowerID)
	{
		SelectedMetas.selected_kingdom = World.world.kingdoms.getRandom();
		return false;
	}

	public static bool whirlwind(BaseSimObject pSelf, WorldTile pTile)
	{
		World.world.applyForceOnTile(pTile, 10, 3f, pForceOut: false, 0, null, pSelf);
		return true;
	}

	public static void removeUnit(Actor pActor)
	{
		pActor.removeByMetamorphosis();
	}

	public static bool breakBones(BaseSimObject pSelf, BaseSimObject pTarget, WorldTile pTile)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		if (pTarget.isActor())
		{
			pTarget.a.addInjuryTrait("crippled");
		}
		return true;
	}

	public static bool restoreMana(WorldTile pTile, Actor pSelf)
	{
		if (pSelf.isManaFull())
		{
			return false;
		}
		int tAdd = (int)((float)pSelf.getMaxMana() * 0.01f);
		pSelf.addMana(tAdd);
		return true;
	}

	public static bool restoreStamina(WorldTile pTile, Actor pSelf)
	{
		if (pSelf.isStaminaFull())
		{
			return false;
		}
		int tAdd = (int)((float)pSelf.getMaxStamina() * 0.01f);
		pSelf.addStamina(tAdd);
		return true;
	}

	public static bool restoreFullStats(NanoObject pTarget, BaseAugmentationAsset pTrait)
	{
		if (pTarget.isRekt())
		{
			return false;
		}
		((Actor)pTarget).event_full_stats = true;
		return true;
	}

	public static bool forcedKingdomAdd(NanoObject pTarget, BaseAugmentationAsset pTrait)
	{
		if (!pTarget.isAlive())
		{
			return false;
		}
		ActorTrait tTrait = (ActorTrait)pTrait;
		Actor tActor = (Actor)pTarget;
		if (tActor.asset.is_boat)
		{
			tActor.getHitFullHealth(AttackType.Explosion);
			return false;
		}
		tActor.applyForcedKingdomTrait();
		tActor.setForcedKingdom(tTrait.getForcedKingdom());
		return true;
	}

	public static bool forcedKingdomEffectRemove(NanoObject pTarget, BaseAugmentationAsset pTrait)
	{
		if (pTarget.isRekt())
		{
			return false;
		}
		((Actor)pTarget).setDefaultKingdom();
		return true;
	}

	public static bool madnessEffectLoad(NanoObject pTarget, BaseAugmentationAsset pTrait)
	{
		if (pTarget.isRekt())
		{
			return false;
		}
		((Actor)pTarget).setForcedKingdom(((ActorTrait)pTrait).getForcedKingdom());
		return true;
	}

	public static bool tryToMakeBuildingAlive(Building pBuilding)
	{
		if (!pBuilding.isAlive())
		{
			return false;
		}
		if (pBuilding.isRuin())
		{
			return false;
		}
		if (pBuilding.isUnderConstruction())
		{
			return false;
		}
		if (!pBuilding.asset.can_be_living_house)
		{
			return false;
		}
		Actor actor = World.world.units.createNewUnit("living_house", pBuilding.current_tile);
		actor.data.set("special_sprite_id", pBuilding.asset.id);
		actor.data.set("special_sprite_index", pBuilding.animData_index);
		actor.data.created_time = pBuilding.data.created_time;
		pBuilding.removeBuildingFinal();
		actor.startColorEffect();
		return true;
	}

	public static bool tryToMakeFloraAlive(Building pBuilding, bool pFullyGrownOnly = true)
	{
		if (!pBuilding.isAlive())
		{
			return false;
		}
		if (pBuilding.isRuin())
		{
			return false;
		}
		if (!pBuilding.asset.can_be_living_plant)
		{
			return false;
		}
		if (pBuilding.chopped)
		{
			return false;
		}
		if (pBuilding.isUnderConstruction())
		{
			return false;
		}
		if (pFullyGrownOnly && !pBuilding.isFullyGrown())
		{
			return false;
		}
		Actor actor = World.world.units.createNewUnit("living_plants", pBuilding.current_tile, pMiracleSpawn: false, 0f, null, null, pSpawnWithItems: false);
		actor.data.set("special_sprite_id", pBuilding.asset.id);
		actor.data.set("special_sprite_index", pBuilding.animData_index);
		actor.data.created_time = pBuilding.data.created_time;
		pBuilding.removeBuildingFinal();
		actor.startColorEffect();
		return true;
	}

	public static void growRandomVegetation(WorldTile pTile, BiomeAsset pBiomeAsset)
	{
		switch (Randy.randomInt(0, 3))
		{
		case 0:
			if (pBiomeAsset.grow_type_selector_trees != null)
			{
				BuildingActions.tryGrowVegetationRandom(pTile, VegetationType.Trees);
			}
			break;
		case 1:
			if (pBiomeAsset.grow_type_selector_plants != null)
			{
				BuildingActions.tryGrowVegetationRandom(pTile, VegetationType.Plants);
			}
			break;
		case 2:
			if (pBiomeAsset.grow_type_selector_bushes != null)
			{
				BuildingActions.tryGrowVegetationRandom(pTile, VegetationType.Bushes);
			}
			break;
		}
	}

	private static NanoObject getNanoObjectFromTile(WorldTile pTile, MetaTypeAsset pMetaTypeAsset)
	{
		if (pTile == null)
		{
			return null;
		}
		NanoObject tMetaObject = pMetaTypeAsset.tile_get_metaobject(pTile.zone, pMetaTypeAsset.getZoneOptionState()) as NanoObject;
		if (tMetaObject.isRekt())
		{
			return null;
		}
		return tMetaObject;
	}
}
