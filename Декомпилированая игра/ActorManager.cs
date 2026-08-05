using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ai;
using ai.behaviours;
using UnityEngine;

public class ActorManager : SimSystemManager<Actor, ActorData>
{
	private JobManagerActors _job_manager;

	public readonly ActorRenderData render_data = new ActorRenderData(4096);

	public readonly ActorVisibleDataArray visible_units_avatars = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units_alive = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units_with_status = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units_with_favorite = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units_with_banner = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units_just_ate = new ActorVisibleDataArray();

	public readonly ActorVisibleDataArray visible_units_socialize = new ActorVisibleDataArray();

	private double _timestamp_sleeping_units;

	public readonly List<Actor> cached_sleeping_units = new List<Actor>();

	private readonly List<ActorVisibleDataArray> _unit_visible_lists = new List<ActorVisibleDataArray>();

	public bool have_dying_units;

	public readonly List<Actor> units_only_wild = new List<Actor>();

	public readonly List<Actor> units_only_civ = new List<Actor>();

	public readonly List<Actor> units_only_alive = new List<Actor>();

	public readonly List<Actor> units_only_dying = new List<Actor>();

	public ActorManager()
	{
		type_id = "unit";
		_job_manager = new JobManagerActors("actors");
		_unit_visible_lists.Add(visible_units);
		_unit_visible_lists.Add(visible_units_avatars);
		_unit_visible_lists.Add(visible_units_alive);
		_unit_visible_lists.Add(visible_units_with_status);
		_unit_visible_lists.Add(visible_units_with_favorite);
		_unit_visible_lists.Add(visible_units_with_banner);
		_unit_visible_lists.Add(visible_units_just_ate);
		_unit_visible_lists.Add(visible_units_socialize);
	}

	public void prepareForMetaChecks()
	{
		units_only_wild.Clear();
		units_only_alive.Clear();
		units_only_dying.Clear();
		units_only_civ.Clear();
		have_dying_units = false;
		List<Actor> tActorList = getSimpleList();
		for (int i = 0; i < tActorList.Count; i++)
		{
			Actor tUnit = tActorList[i];
			if (tUnit.isAlive())
			{
				if (tUnit.kingdom.wild)
				{
					units_only_wild.Add(tUnit);
				}
				else
				{
					units_only_civ.Add(tUnit);
				}
				units_only_alive.Add(tUnit);
			}
			else
			{
				units_only_dying.Add(tUnit);
				have_dying_units = true;
			}
		}
	}

	public void calculateVisibleActors()
	{
		Bench.bench("actors_prepare_lists", "game_total");
		clearLists();
		prepareLists();
		Bench.benchEnd("actors_prepare_lists", "game_total", pSaveCounter: false, 0L);
		Bench.bench("actors_fill_visible", "game_total");
		fillVisibleObjects();
		Bench.benchEnd("actors_fill_visible", "game_total", pSaveCounter: false, 0L);
		Bench.bench("actors_precalc_render_data_parallel", "game_total");
		precalculateRenderDataParallel();
		Bench.benchEnd("actors_precalc_render_data_parallel", "game_total", pSaveCounter: false, 0L);
		Bench.bench("actors_precalc_render_data_normal", "game_total");
		precalculateRenderDataNormal();
		Bench.benchEnd("actors_precalc_render_data_normal", "game_total", pSaveCounter: false, 0L);
	}

	private void precalculateRenderDataParallel()
	{
		int tDebugItemScale = ((!DebugConfig.isOn(DebugOption.RenderBigItems)) ? 1 : 10);
		bool tShouldRenderUnitShadows = World.world.quality_changer.shouldRenderUnitShadows();
		int tTotalVisibleObjects = visible_units.count;
		Actor[] tArray = visible_units.array;
		int tDynamicBatchSize = 256;
		int tTotalBatches = ParallelHelper.calcTotalBatches(tTotalVisibleObjects, tDynamicBatchSize);
		Parallel.For(0, tTotalBatches, World.world.parallel_options, delegate(int pBatchIndex)
		{
			int num = ParallelHelper.calculateBatchBeg(pBatchIndex, tDynamicBatchSize);
			int num2 = ParallelHelper.calculateBatchEnd(num, tDynamicBatchSize, tTotalVisibleObjects);
			for (int i = num; i < num2; i++)
			{
				Actor actor = tArray[i];
				Vector3 current_scale = actor.current_scale;
				Vector3 vector = actor.updateRotation();
				Vector3 vector2 = actor.updatePos();
				bool flag = actor.checkHasRenderedItem();
				bool flag2 = !actor.asset.ignore_generic_render;
				Sprite sprite;
				if (flag)
				{
					Sprite renderedItemSprite = actor.getRenderedItemSprite();
					IHandRenderer cachedHandRendererAsset = actor.getCachedHandRendererAsset();
					int pColorID = -900000;
					if (cachedHandRendererAsset.is_colored)
					{
						pColorID = actor.kingdom.getColor().GetHashCode();
					}
					sprite = DynamicSprites.getCachedAtlasItemSprite(DynamicSprites.getItemSpriteID(renderedItemSprite, pColorID), renderedItemSprite);
				}
				else
				{
					sprite = null;
				}
				render_data.positions[i] = vector2;
				render_data.scales[i] = current_scale;
				render_data.rotations[i] = vector;
				render_data.flip_x_states[i] = actor.flip;
				render_data.colors[i] = actor.color;
				render_data.has_normal_render[i] = flag2;
				render_data.has_item[i] = flag;
				render_data.item_sprites[i] = sprite;
				AnimationFrameData animationFrameData = actor.getAnimationFrameData();
				bool flag3 = false;
				if (tShouldRenderUnitShadows && actor.show_shadow)
				{
					ActorTextureSubAsset actorTextureSubAsset = ((!actor.hasSubspecies() || !actor.subspecies.has_mutation_reskin) ? actor.asset.texture_asset : actor.subspecies.mutation_skin_asset.texture_asset);
					flag3 = actorTextureSubAsset.shadow;
					if (actorTextureSubAsset.shadow)
					{
						Vector2 vector3;
						if (actor.isEgg())
						{
							render_data.shadow_sprites[i] = actorTextureSubAsset.shadow_sprite_egg;
							vector3 = actorTextureSubAsset.shadow_size_egg;
						}
						else if (actor.isBaby())
						{
							render_data.shadow_sprites[i] = actorTextureSubAsset.shadow_sprite_baby;
							vector3 = actorTextureSubAsset.shadow_size_baby;
						}
						else
						{
							render_data.shadow_sprites[i] = actorTextureSubAsset.shadow_sprite;
							vector3 = actorTextureSubAsset.shadow_size;
						}
						vector3 *= (Vector2)current_scale;
						int num3 = (actor.flip ? 1 : (-1));
						float num4 = vector3.x / 2f;
						float num5 = vector3.y * 0.6f;
						float num6 = Mathf.Abs(vector.z);
						Vector2 current_shadow_position = actor.current_shadow_position;
						current_shadow_position.x += num4 * (vector.z * (float)num3) / 90f;
						current_shadow_position.y -= num5 * num6 / 90f;
						render_data.shadow_position[i] = current_shadow_position;
						if (animationFrameData != null && animationFrameData.size_unit != default(Vector2))
						{
							float b = (animationFrameData.size_unit * current_scale).y / vector3.x * current_scale.x;
							float x = Mathf.Lerp(current_scale.x, b, num6 / 90f);
							render_data.shadow_scales[i] = new Vector2(x, current_scale.y);
						}
						else
						{
							render_data.shadow_scales[i] = current_scale;
						}
					}
				}
				render_data.shadows[i] = flag3;
				if (flag2)
				{
					if (actor.canParallelSetColoredSprite())
					{
						Sprite sprite2 = actor.calculateMainSprite();
						render_data.main_sprites[i] = sprite2;
						if (actor.hasColoredSprite())
						{
							if (!actor.isColoredSpriteNeedsCheck(sprite2))
							{
								render_data.main_sprite_colored[i] = actor.getLastColoredSprite();
							}
							else
							{
								render_data.main_sprite_colored[i] = null;
							}
						}
						else
						{
							render_data.main_sprite_colored[i] = sprite2;
						}
					}
					else
					{
						render_data.main_sprites[i] = null;
						render_data.main_sprite_colored[i] = null;
					}
					if (flag)
					{
						render_data.item_scale[i] = current_scale * tDebugItemScale;
						float num7 = 0f;
						float num8 = 0f;
						if (animationFrameData != null)
						{
							num7 = animationFrameData.pos_item.x;
							num8 = animationFrameData.pos_item.y;
						}
						float x2 = vector2.x + num7 * current_scale.x;
						float y = vector2.y + num8 * current_scale.y;
						float z = -0.01f + num8 * current_scale.y;
						Vector3 point = new Vector3(x2, y);
						Vector3 angles = vector;
						if (angles.y != 0f || angles.z != 0f)
						{
							Vector3 pivot = new Vector3(vector2.x, vector2.y, 0f);
							point = Toolbox.RotatePointAroundPivot(ref point, ref pivot, ref angles);
						}
						point.z = z;
						render_data.item_pos[i] = point;
					}
				}
			}
		});
	}

	private void precalculateRenderDataNormal()
	{
		ActorRenderData tRenderData = render_data;
		int tTotalVisibleObjects = visible_units.count;
		Actor[] tArray = visible_units.array;
		for (int i = 0; i < tTotalVisibleObjects; i++)
		{
			Actor tActor = tArray[i];
			if (tRenderData.has_normal_render[i] && (object)tRenderData.main_sprite_colored[i] == null)
			{
				Sprite tMainSprite = tRenderData.main_sprites[i];
				if ((object)tMainSprite == null)
				{
					tMainSprite = tActor.calculateMainSprite();
				}
				tRenderData.main_sprite_colored[i] = tActor.calculateColoredSprite(tMainSprite);
			}
		}
	}

	private void fillVisibleObjects()
	{
		prepareArray();
		Actor[] tUnits = getSimpleArray();
		int tCountTotal = Count;
		bool tRenderNormalUnits = MapBox.isRenderGameplay();
		int tCountVisible = 0;
		int tCountVisibleAlive = 0;
		Actor[] tVisible = visible_units.array;
		Actor[] tVisibleAlive = visible_units_alive.array;
		for (int i = 0; i < tCountTotal; i++)
		{
			Actor tActor = tUnits[i];
			ActorAsset tActorAsset = tActor.asset;
			TileZone tZone = tActor.current_tile.zone;
			if (tActorAsset.has_avatar_prefab)
			{
				visible_units_avatars.array[visible_units_avatars.count++] = tActor;
			}
			if (tActor.isFavorite() && !tActorAsset.hide_favorite_icon && tZone.visible && !ControllableUnit.isControllingUnit(tActor))
			{
				visible_units_with_favorite.array[visible_units_with_favorite.count++] = tActor;
			}
			if (!tZone.visible || !tRenderNormalUnits || !tActor.is_visible)
			{
				continue;
			}
			tVisible[tCountVisible++] = tActor;
			if (tActor.isAlive())
			{
				tVisibleAlive[tCountVisibleAlive++] = tActor;
				if (tActor.is_army_captain)
				{
					visible_units_with_banner.array[visible_units_with_banner.count++] = tActor;
				}
				if (tActorAsset.render_status_effects && tActor.hasAnyStatusEffectToRender())
				{
					visible_units_with_status.array[visible_units_with_status.count++] = tActor;
				}
				if (tActor.timestamp_session_ate_food > 0.0)
				{
					visible_units_just_ate.array[visible_units_just_ate.count++] = tActor;
				}
				BehaviourActionActor tActorAction = tActor.ai.action;
				if (tActorAction != null && tActorAction.socialize)
				{
					visible_units_socialize.array[visible_units_socialize.count++] = tActor;
				}
				else if (tActor.is_forced_socialize_icon && !tActor.is_moving && !tActor.isLying() && tActor.isAttackReady() && Date.getMonthsSince(tActor.is_forced_socialize_timestamp) < 1)
				{
					visible_units_socialize.array[visible_units_socialize.count++] = tActor;
				}
			}
		}
		visible_units.count = tCountVisible;
		visible_units_alive.count = tCountVisibleAlive;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		Bench.bench("actors", "game_total");
		checkContainer();
		_job_manager.updateBase(pElapsed);
		checkContainer();
		Bench.benchEnd("actors", "game_total", pSaveCounter: false, 0L);
	}

	private void checkOverrideUnitShooting()
	{
		if (!DebugConfig.isOn(DebugOption.OverrideUnitShooting) || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Vector2 tCursorPos = World.world.getMousePos();
		WorldTile tCursorTile = World.world.getMouseTilePos();
		Actor tCursorActor = World.world.getActorNearCursor();
		if (tCursorTile == null)
		{
			return;
		}
		using IEnumerator<Actor> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Actor tActor = enumerator.Current;
			if (tActor != tCursorActor && tActor.isAlive() && tActor.hasRangeAttack())
			{
				AttackData tAttackData = new AttackData(tActor, pKingdom: tActor.kingdom, pInitiatorPosition: tActor.current_position, pHitTile: tCursorTile, pHitPosition: tCursorPos, pTarget: null, pAttackType: AttackType.Weapon, pMetallicWeapon: false, pSkipShake: true, pProjectile: false, pProjectileID: tActor.getWeaponAsset().projectile);
				CombatActionLibrary.combat_attack_range.action(tAttackData);
			}
		}
	}

	protected override void destroyObject(Actor pActor)
	{
		base.destroyObject(pActor);
		if (pActor.hasKingdom())
		{
			pActor.setKingdom(null);
		}
		if (pActor.hasSubspecies())
		{
			pActor.setSubspecies(null);
		}
		if (pActor.tile_target != null)
		{
			pActor.clearTileTarget();
		}
		pActor.asset.units.Remove(pActor);
		removeObject(pActor);
		_job_manager.removeObject(pActor, pActor.batch);
		if (pActor.avatar != null)
		{
			UnityEngine.Object.Destroy(pActor.avatar);
			pActor.avatar = null;
		}
		if (pActor.idle_loop_sound != null)
		{
			pActor.idle_loop_sound.stop();
		}
	}

	internal override void scheduleDestroyOnPlay(Actor pObject)
	{
		triggerActionsOnRemove(pObject);
		base.scheduleDestroyOnPlay(pObject);
	}

	private void triggerActionsOnRemove(Actor pActor)
	{
		foreach (ActorTrait tTrait in pActor.traits)
		{
			tTrait.action_on_object_remove?.Invoke(pActor, tTrait);
		}
	}

	public override void loadFromSave(List<ActorData> pList)
	{
		base.loadFromSave(pList);
		checkContainer();
	}

	public Actor evolutionEvent(Actor pTargetActor, bool pWithBiomeEffect, bool pAscension)
	{
		Subspecies tOriginalSubspecies = pTargetActor.subspecies;
		bool tNewSubspecies = false;
		Subspecies tEvolvedSubspecies = null;
		string tNewActorAssetId = pTargetActor.asset.id;
		if (tOriginalSubspecies.hasEvolvedIntoForm() && !pAscension)
		{
			tEvolvedSubspecies = tOriginalSubspecies.getEvolvedInto();
			if (tEvolvedSubspecies != null)
			{
				tNewActorAssetId = tEvolvedSubspecies.getActorAsset().id;
			}
		}
		if (tEvolvedSubspecies == null)
		{
			bool tNewAsset = false;
			if (pTargetActor.asset.can_evolve_into_new_species)
			{
				tNewAsset = tOriginalSubspecies.isSapient() || Randy.randomBool();
				if (tNewAsset)
				{
					tNewActorAssetId = pTargetActor.asset.evolution_id;
				}
			}
			if (!tNewAsset)
			{
				Subspecies subspecies = World.world.subspecies.newSpecies(pTargetActor.asset, pTargetActor.current_tile, pMutation: true);
				tNewSubspecies = true;
				subspecies.mutateFrom(tOriginalSubspecies);
				tEvolvedSubspecies = subspecies;
			}
		}
		if (tEvolvedSubspecies == null)
		{
			tEvolvedSubspecies = World.world.subspecies.newSpecies(AssetManager.actor_library.get(tNewActorAssetId), pTargetActor.current_tile);
			tNewSubspecies = true;
			tEvolvedSubspecies.mutateFrom(tOriginalSubspecies);
		}
		if (tNewSubspecies)
		{
			tEvolvedSubspecies.addTrait("uplifted");
			tEvolvedSubspecies.makeSapient();
			tEvolvedSubspecies.data.biome_variant = tOriginalSubspecies.data.biome_variant;
		}
		ActorAsset tNewActorAsset = AssetManager.actor_library.get(tNewActorAssetId);
		pTargetActor.setAsset(tNewActorAsset);
		pTargetActor.setSubspecies(tEvolvedSubspecies);
		tEvolvedSubspecies.data.parent_subspecies = tOriginalSubspecies.id;
		if (pAscension)
		{
			string tName = tEvolvedSubspecies.name;
			if (!tName.Contains("Ascentus"))
			{
				tName += " Ascentus";
				tEvolvedSubspecies.setName(tName);
			}
		}
		else
		{
			tOriginalSubspecies.setEvolutionSubspecies(tEvolvedSubspecies);
		}
		if (pWithBiomeEffect && Randy.randomChance(0.1f))
		{
			BiomeAsset tBiome = pTargetActor.current_tile.getBiome();
			if (tBiome != null && tBiome.evolution_trait_subspecies != null && tBiome.evolution_trait_subspecies.Count > 0)
			{
				SubspeciesTrait tTrait = AssetManager.subspecies_traits.get(tBiome.evolution_trait_subspecies.GetRandom());
				if (tTrait != null)
				{
					tEvolvedSubspecies.addTrait(tTrait);
				}
			}
		}
		pTargetActor.afterEvolutionEvents();
		return pTargetActor;
	}

	public bool cloneUnit(Actor pCloneFrom, WorldTile pTileTarget = null)
	{
		if (pCloneFrom == null)
		{
			return false;
		}
		if (!pCloneFrom.asset.can_be_cloned)
		{
			return false;
		}
		pCloneFrom.prepareForSave();
		ActorData tOriginalData = pCloneFrom.data;
		string tName = pCloneFrom.getName();
		ActorData tNewBabyData = new ActorData();
		ActorTool.copyImportantData(tOriginalData, tNewBabyData, pCopyAge: true);
		tNewBabyData.created_time = World.world.getCurWorldTime();
		tNewBabyData.id = World.world.map_stats.getNextId("unit");
		tNewBabyData.name = tName;
		tNewBabyData.custom_name = tOriginalData.custom_name;
		tNewBabyData.age_overgrowth = tOriginalData.getAge();
		tNewBabyData.parent_id_1 = tOriginalData.id;
		pCloneFrom.increaseBirths();
		if (pTileTarget == null)
		{
			pTileTarget = pCloneFrom.current_tile.neighboursAll.GetRandom();
		}
		tNewBabyData.x = pTileTarget.x;
		tNewBabyData.y = pTileTarget.y;
		Actor tCloneActor = World.world.units.loadObject(tNewBabyData);
		tCloneActor.created_time_unscaled = Time.time;
		tCloneActor.addTrait("fragile_health");
		foreach (ActorTrait tTrait in pCloneFrom.getTraits())
		{
			tCloneActor.addTrait(tTrait);
		}
		tCloneActor.addTrait("miracle_born");
		if (!pCloneFrom.hasFamily() && pCloneFrom.asset.create_family_at_spawn)
		{
			World.world.families.newFamily(pCloneFrom, pCloneFrom.current_tile, null);
		}
		tCloneActor.data.cloneCustomDataFrom(pCloneFrom.data);
		tCloneActor.setReligion(pCloneFrom.religion);
		tCloneActor.setClan(pCloneFrom.clan);
		tCloneActor.setCulture(pCloneFrom.culture);
		tCloneActor.setSubspecies(pCloneFrom.subspecies);
		tCloneActor.joinLanguage(pCloneFrom.language);
		tCloneActor.setFamily(pCloneFrom.family);
		tCloneActor.setHealth(tOriginalData.health, pClamp: false);
		tCloneActor.setMana(tOriginalData.mana, pClamp: false);
		tCloneActor.setStamina(tOriginalData.stamina, pClamp: false);
		tCloneActor.setHappiness(tOriginalData.happiness, pClamp: false);
		tCloneActor.setNutrition(tOriginalData.nutrition, pClamp: false);
		tCloneActor.addTrait("clone");
		if (tOriginalData.saved_items != null)
		{
			foreach (long tID in tOriginalData.saved_items)
			{
				Item tItem = World.world.items.get(tID);
				if (tItem != null)
				{
					Item tNewItem = World.world.items.generateItem(tItem.getAsset(), null, null, 1, pCloneFrom);
					tNewItem.data.modifiers.Clear();
					tNewItem.data.modifiers.AddRange(tItem.data.modifiers);
					tNewItem.data.modifiers.Remove("eternal");
					tNewItem.initItem();
					tCloneActor.equipment.setItem(tNewItem, tCloneActor);
				}
			}
		}
		tCloneActor.applyRandomForce();
		if (tCloneActor.isRendered())
		{
			EffectsLibrary.spawn("fx_spawn", pTileTarget);
		}
		if (tCloneActor.asset.has_sound_spawn)
		{
			MusicBox.playSound(tCloneActor.asset.sound_spawn, pTileTarget);
		}
		return true;
	}

	public Actor createNewUnit(string pStatsID, WorldTile pTile, bool pMiracleSpawn = false, float pSpawnHeight = 0f, Subspecies pSubspecies = null, Subspecies pSubspeciesMutateFrom = null, bool pSpawnWithItems = true, bool pAdultAge = false, bool pGiveOwnerlessItems = false, bool pSapientSubspecies = false)
	{
		ActorAsset tAsset = AssetManager.actor_library.get(pStatsID);
		if (tAsset == null)
		{
			return null;
		}
		Actor tActor = newObject();
		tActor.setAsset(tAsset);
		if (!pSubspecies.isRekt())
		{
			tActor.setSubspecies(pSubspecies);
		}
		else
		{
			checkNewSpecies(tAsset, pTile, tActor, out var tClosestSameActor, pGlobalSearch: false, pSapientSubspecies, pSubspeciesMutateFrom);
			if (pMiracleSpawn && tClosestSameActor != null)
			{
				if (tClosestSameActor.hasCulture())
				{
					tActor.setCulture(tClosestSameActor.culture);
				}
				if (tClosestSameActor.hasReligion())
				{
					tActor.setReligion(tClosestSameActor.religion);
				}
				if (tClosestSameActor.hasLanguage())
				{
					tActor.setLanguage(tClosestSameActor.language);
				}
			}
		}
		addRandomTraitFromBiomeToActor(tActor, pTile);
		finalizeActor(pStatsID, tActor, pTile, pSpawnHeight);
		if (pMiracleSpawn || pAdultAge)
		{
			if (pMiracleSpawn)
			{
				tActor.addTrait("miracle_born");
			}
			if (tActor.hasSubspecies())
			{
				tActor.data.age_overgrowth = (int)Math.Ceiling(tActor.subspecies.age_breeding);
			}
			else
			{
				tActor.data.age_overgrowth = tAsset.age_spawn;
			}
			if (HotkeyLibrary.isHoldingAlt())
			{
				tActor.data.age_overgrowth = 0;
			}
		}
		tActor.newCreature();
		if (pSpawnWithItems)
		{
			tActor.generateDefaultSpawnWeapons(pGiveOwnerlessItems);
		}
		tActor.clearSprites();
		return tActor;
	}

	private void finalizeActor(string pStats, Actor pActor, WorldTile pTile, float pZHeight = 0f)
	{
		ActorAsset tAsset = AssetManager.actor_library.get(pStats);
		pActor.setAsset(tAsset);
		ActorData tData = pActor.data;
		pActor.spawnOn(pTile, pZHeight);
		if (tData.subspecies.hasValue())
		{
			pActor.setSubspecies(World.world.subspecies.get(tData.subspecies));
		}
		if (tData.family.hasValue())
		{
			pActor.setFamily(World.world.families.get(tData.family));
		}
		if (tData.language.hasValue())
		{
			pActor.setLanguage(World.world.languages.get(tData.language));
		}
		if (tData.plot.hasValue())
		{
			pActor.setPlot(World.world.plots.get(tData.plot));
		}
		if (tData.religion.hasValue())
		{
			pActor.setReligion(World.world.religions.get(tData.religion));
		}
		if (tData.clan.hasValue())
		{
			pActor.setClan(World.world.clans.get(tData.clan));
		}
		if (tData.culture.hasValue())
		{
			pActor.setCulture(World.world.cultures.get(tData.culture));
		}
		if (tData.army.hasValue())
		{
			pActor.setArmy(World.world.armies.get(tData.army));
		}
		pActor.create();
		pActor.checkDefaultKingdom();
		pActor.checkDefaultProfession();
		pActor.updateStats();
		if (pActor.asset.can_be_killed_by_stuff)
		{
			pActor.batch.c_main_tile_action.Add(pActor);
		}
	}

	public Actor createBabyActorFromData(ActorData pData, WorldTile pTile, City pCity)
	{
		ActorAsset tStats = AssetManager.actor_library.get(pData.asset_id);
		Actor tActor = base.loadObject(pData);
		tActor.setData(pData);
		tActor.created_time_unscaled = Time.time;
		finalizeActor(tStats.id, tActor, pTile);
		return tActor;
	}

	public Actor spawnNewUnitByPlayer(string pStatsID, WorldTile pTile, bool pSpawnSound = false, bool pMiracleSpawn = false, float pSpawnHeight = 6f, Subspecies pSubspecies = null)
	{
		Actor tActor = spawnNewUnit(pStatsID, pTile, pSpawnSound, pMiracleSpawn, pSpawnHeight, pSubspecies, pGiveOwnerlessItems: true);
		if (tActor.current_zone.hasCity() && tActor.isSapient())
		{
			City tCity = tActor.current_zone.city;
			if (!tCity.isNeutral() && tCity.isPossibleToJoin(tActor))
			{
				tActor.joinCity(tCity);
			}
		}
		return tActor;
	}

	public Actor spawnNewUnit(string pActorAssetID, WorldTile pTile, bool pSpawnSound = false, bool pMiracleSpawn = false, float pSpawnHeight = 6f, Subspecies pSubspecies = null, bool pGiveOwnerlessItems = false, bool pAdultAge = false)
	{
		bool pGiveOwnerlessItems2 = pGiveOwnerlessItems;
		Actor tUnit = createNewUnit(pActorAssetID, pTile, pMiracleSpawn, pSpawnHeight, pSubspecies, null, pSpawnWithItems: true, pAdultAge, pGiveOwnerlessItems2);
		if (pSpawnSound && tUnit.asset.has_sound_spawn)
		{
			MusicBox.playSound(tUnit.asset.sound_spawn, pTile.pos.x, pTile.pos.y);
		}
		if (tUnit.kingdom == null)
		{
			Kingdom tNomadKingdom = World.world.kingdoms_wild.get(tUnit.asset.kingdom_id_wild);
			tUnit.setKingdom(tNomadKingdom);
		}
		tUnit.setStatsDirty();
		tUnit.setNutrition(SimGlobals.m.nutrition_level_on_spawn);
		return tUnit;
	}

	private void checkNewSpecies(ActorAsset pAsset, WorldTile pTile, Actor pActor, out Actor pClosestActor, bool pGlobalSearch = false, bool pLookForSapientSubspecies = false, Subspecies pSubspeciesMutateFrom = null)
	{
		pClosestActor = null;
		if (!pAsset.can_have_subspecies)
		{
			return;
		}
		Subspecies tSubspecies = null;
		if (pGlobalSearch)
		{
			foreach (Subspecies tExistingSubspecies in World.world.subspecies)
			{
				if (tExistingSubspecies.isSpecies(pAsset.id))
				{
					tSubspecies = tExistingSubspecies;
					break;
				}
			}
		}
		if (tSubspecies == null)
		{
			tSubspecies = World.world.subspecies.getNearbySpecies(pAsset, pTile, out var tClosestActor, pLookForSapientSubspecies, pStopAtFirst: true);
			pClosestActor = tClosestActor;
		}
		if (tSubspecies == null)
		{
			tSubspecies = World.world.subspecies.newSpecies(pAsset, pTile);
			if (pSubspeciesMutateFrom != null)
			{
				tSubspecies.mutateFrom(pSubspeciesMutateFrom);
			}
			tSubspecies.forceRecalcBaseStats();
		}
		pActor.setSubspecies(tSubspecies);
		pActor.event_full_stats = true;
		pActor.setStatsDirty();
	}

	public ActorTrait addRandomTraitFromBiomeToActor(Actor pActor, WorldTile pTile)
	{
		if (!pTile.Type.is_biome)
		{
			return null;
		}
		BiomeAsset tBiomeAsset = pTile.Type.biome_asset;
		List<string> spawn_trait_actor = tBiomeAsset.spawn_trait_actor;
		if (spawn_trait_actor != null && spawn_trait_actor.Count > 0 && Randy.randomBool())
		{
			string tRandomTraitID = tBiomeAsset.spawn_trait_actor.GetRandom();
			ActorTrait tTrait = AssetManager.traits.get(tRandomTraitID);
			pActor.addTrait(tTrait);
			return tTrait;
		}
		return null;
	}

	public override Actor loadObject(ActorData pData)
	{
		if (dict.ContainsKey(pData.id))
		{
			Debug.Log("Trying to load unit with same ID, that already is loaded. " + pData.id);
			return null;
		}
		WorldTile tTile = World.world.GetTile(pData.x, pData.y);
		if (tTile == null)
		{
			return null;
		}
		ActorAsset tAsset = AssetManager.actor_library.get(pData.asset_id);
		if (tAsset == null)
		{
			return null;
		}
		int tSavedHealth = pData.health;
		int tSavedNutrition = pData.nutrition;
		int tSavedStamina = pData.stamina;
		int tSavedMana = pData.mana;
		Actor tActor = base.loadObject(pData);
		tActor.setData(pData);
		finalizeActor(tAsset.id, tActor, tTile);
		if (tActor.canUseItems())
		{
			tActor.equipment.load(pData.saved_items, tActor);
		}
		if (tActor.isSapient())
		{
			tActor.reloadInventory();
		}
		tActor.loadFromSave();
		tActor.updateStats();
		tActor.setHealth(tSavedHealth);
		tActor.setNutrition(tSavedNutrition);
		tActor.setStamina(tSavedStamina);
		tActor.setMana(tSavedMana);
		if (tActor.asset.can_have_subspecies && !tActor.hasSubspecies())
		{
			checkNewSpecies(tActor.asset, tActor.current_tile, tActor, out var _, pGlobalSearch: true);
		}
		tActor.makeWait(Randy.randomFloat(0.1f, 2f));
		return tActor;
	}

	protected override void addObject(Actor pObject)
	{
		base.addObject(pObject);
		_job_manager.addNewObject(pObject);
	}

	private void clearLists()
	{
		for (int i = 0; i < _unit_visible_lists.Count; i++)
		{
			_unit_visible_lists[i].count = 0;
		}
	}

	private void prepareLists()
	{
		for (int i = 0; i < _unit_visible_lists.Count; i++)
		{
			_unit_visible_lists[i].prepare(Count);
		}
		render_data.checkSize(Count);
		checkContainer();
	}

	public override void clear()
	{
		_job_manager.clear();
		cached_sleeping_units.Clear();
		clearLists();
		checkContainer();
		scheduleDestroyAllOnWorldClear();
		checkObjectsToDestroy();
		base.clear();
	}

	public void debugJobManager(DebugTool pTool)
	{
		_job_manager.debug(pTool);
	}

	public JobManagerActors getJobManager()
	{
		return _job_manager;
	}

	public void checkSleepingUnits()
	{
		if (World.world.getWorldTimeElapsedSince(_timestamp_sleeping_units) < 10f)
		{
			return;
		}
		cached_sleeping_units.Clear();
		_timestamp_sleeping_units = World.world.getCurWorldTime();
		foreach (Status tStatus in World.world.statuses.list.LoopRandom())
		{
			if (tStatus.is_finished || tStatus.asset.id != "sleeping")
			{
				continue;
			}
			Actor tActor = tStatus.sim_object.a;
			if (tActor.isAlive())
			{
				cached_sleeping_units.Add(tActor);
				if (cached_sleeping_units.Count > 10)
				{
					break;
				}
			}
		}
	}
}
