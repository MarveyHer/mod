using System.Collections.Generic;
using UnityEngine;

public static class MapAction
{
	private static List<WorldTile> temp_list_tiles_road_path = new List<WorldTile>();

	private static List<WorldTile> temp_list_tiles_road_tiles_to_build = new List<WorldTile>();

	public static void checkAcidTerraform(WorldTile pTile)
	{
		if (pTile.isTemporaryFrozen())
		{
			pTile.unfreeze(99);
		}
		if (pTile.top_type != null && pTile.top_type.wasteland)
		{
			return;
		}
		if (pTile.top_type != null)
		{
			decreaseTile(pTile, pDamage: true);
		}
		else if (pTile.Type.ground)
		{
			if (pTile.isTileRank(TileRank.Low))
			{
				terraformTop(pTile, TopTileLibrary.wasteland_low);
			}
			else if (pTile.isTileRank(TileRank.High))
			{
				terraformTop(pTile, TopTileLibrary.wasteland_high);
			}
			AchievementLibrary.lets_not.check();
		}
	}

	public static void terraformMain(WorldTile pTile, TileType pType, bool pSkipTerraform = false)
	{
		terraformTile(pTile, pType, null, TerraformLibrary.flash, pSkipTerraform);
	}

	public static void terraformTop(WorldTile pTile, TopTileType pTopType, bool pSkipTerraform = false)
	{
		terraformTile(pTile, pTile.main_type, pTopType, TerraformLibrary.flash, pSkipTerraform);
	}

	public static void terraformMain(WorldTile pTile, TileType pType, TerraformOptions pOptions, bool pSkipTerraform = false)
	{
		terraformTile(pTile, pType, null, pOptions, pSkipTerraform);
	}

	public static void terraformTop(WorldTile pTile, TopTileType pTopType, TerraformOptions pOptions, bool pSkipTerraform = false)
	{
		terraformTile(pTile, pTile.main_type, pTopType, pOptions, pSkipTerraform);
	}

	public static void terraformTile(WorldTile pTile, TileType pNewTypeMain, TopTileType pTopType, TerraformOptions pOptions = null, bool pSkipTerraform = false)
	{
		if (pOptions == null)
		{
			pOptions = TerraformLibrary.flash;
		}
		TileTypeBase tOldLayerType = pTile.Type;
		TileTypeBase type = pTile.Type;
		if (pOptions.remove_fire)
		{
			pTile.stopFire();
		}
		if (!pSkipTerraform)
		{
			if (pOptions.remove_water && pTile.Type.ocean)
			{
				pNewTypeMain = pTile.Type.decrease_to;
			}
			if (pOptions.remove_top_tile && pTile.top_type != null)
			{
				pNewTypeMain = pTile.Type.decrease_to;
			}
			if (pOptions.remove_roads && pTile.Type.road)
			{
				pNewTypeMain = pTile.Type.decrease_to;
			}
			if (pOptions.remove_frozen && pTile.isTemporaryFrozen())
			{
				pTile.unfreeze(99);
			}
			if (pNewTypeMain != null)
			{
				pTile.setTileTypes(pNewTypeMain, pTopType);
			}
			else
			{
				pTile.setTopTileType(pTopType);
			}
		}
		if (type.can_be_farm != pTile.Type.can_be_farm && !pTile.zone.hasCity())
		{
			World.world.city_zone_helper.city_place_finder.setDirty();
		}
		if ((pTile.burned_stages > 0 && !pTile.Type.can_be_set_on_fire) || pOptions.remove_burned)
		{
			pTile.removeBurn();
		}
		World.world.resetRedrawTimer();
		if (pOptions.remove_borders)
		{
			World.world.checkCityZone(pTile);
		}
		if (pOptions.flash)
		{
			World.world.flash_effects.flashPixel(pTile, 20);
		}
		if (pTile.hasBuilding() && !pTile.building.isRuin() && !pTile.building.asset.isOverlaysBiomeTags(pTile.Type))
		{
			if (pTile.building.asset.has_ruins_graphics)
			{
				pTile.building.startMakingRuins();
			}
			else
			{
				pTile.building.startDestroyBuilding();
			}
		}
		if (pOptions.make_ruins && pTile.hasBuilding())
		{
			Building tBuilding = pTile.building;
			if (tBuilding.asset.has_ruins_graphics)
			{
				tBuilding.startMakingRuins();
			}
			else
			{
				tBuilding.startDestroyBuilding();
			}
			if (!tBuilding.asset.can_be_placed_on_blocks && pTile.Type.rocks)
			{
				tBuilding.startDestroyBuilding();
			}
			if (!tBuilding.asset.can_be_placed_on_liquid && pTile.Type.liquid)
			{
				tBuilding.startDestroyBuilding();
			}
		}
		if (pOptions.destroy_buildings && pTile.hasBuilding())
		{
			bool tDestroy = false;
			if (pOptions.ignore_kingdoms != null)
			{
				tDestroy = true;
				for (int i = 0; i < pOptions.ignore_kingdoms.Length; i++)
				{
					if (!(pOptions.ignore_kingdoms[i] != pTile.building.kingdom?.name))
					{
						tDestroy = false;
						break;
					}
				}
			}
			else if (pOptions.destroy_only == null)
			{
				tDestroy = pOptions.ignore_buildings == null || !pOptions.ignore_buildings.Contains(pTile.building.asset.id);
			}
			else
			{
				tDestroy = false;
				for (int j = 0; j < pOptions.destroy_only.Count; j++)
				{
					if (!(pOptions.destroy_only[j] != pTile.building.asset.group))
					{
						tDestroy = true;
						break;
					}
				}
			}
			if (tDestroy)
			{
				pTile.building.startDestroyBuilding();
			}
		}
		checkTileState(pTile, tOldLayerType);
	}

	public static void checkTileState(WorldTile pTile, TileTypeBase pOldType, bool pForceMapChunk = false)
	{
		if (pOldType.layer_type != pTile.Type.layer_type || pForceMapChunk)
		{
			World.world.map_chunk_manager.setDirty(pTile.chunk);
			MapChunk[] tChunks = pTile.chunk.neighbours_all;
			foreach (MapChunk tNeighbour in tChunks)
			{
				World.world.map_chunk_manager.setDirty(tNeighbour, pRegions: false);
			}
		}
		if (pTile.Type.layer_type != TileLayerType.Ground)
		{
			World.world.checkCityZone(pTile);
		}
		if (pTile.hasBuilding() && !pTile.building.asset.can_be_placed_on_liquid && pTile.Type.ocean)
		{
			pTile.building.startDestroyBuilding();
		}
	}

	public static void setOcean(WorldTile pTile)
	{
		if (pTile.Type.fill_to_ocean != null)
		{
			TileType tType = AssetManager.tiles.get(pTile.Type.fill_to_ocean);
			if (pTile.Type.water_fill_sound != string.Empty)
			{
				MusicBox.playSound(pTile.Type.water_fill_sound, pTile);
			}
			terraformMain(pTile, tType, TerraformLibrary.water_fill);
		}
	}

	public static void decreaseTile(WorldTile pTile, bool pDamage, TerraformOptions pTerraformOption)
	{
		if (checkTileDamageGaiaCovenant(pTile, pDamage))
		{
			if (pTile.isTemporaryFrozen())
			{
				pTile.unfreeze(100);
			}
			else if (pTile.top_type != null)
			{
				terraformTile(pTile, pTile.main_type, null, pTerraformOption);
			}
			else if (pTile.Type.decrease_to != null)
			{
				terraformMain(pTile, pTile.Type.decrease_to, pTerraformOption);
			}
		}
	}

	public static bool checkTileDamageGaiaCovenant(WorldTile pTile, bool pDamage)
	{
		bool tDamageAnyway = pTile.Type.life || pTile.Type.explodable;
		if (pDamage && WorldLawLibrary.world_law_gaias_covenant.isEnabled() && !tDamageAnyway)
		{
			return false;
		}
		return true;
	}

	public static void decreaseTile(WorldTile pTile, bool pDamage, string pTerraformOption = "flash")
	{
		if (checkTileDamageGaiaCovenant(pTile, pDamage))
		{
			decreaseTile(pTile, pDamage, AssetManager.terraform.get(pTerraformOption));
		}
	}

	public static void increaseTile(WorldTile pTile, bool pDamage, string pTerraformOption = "flash")
	{
		if (checkTileDamageGaiaCovenant(pTile, pDamage))
		{
			if (pTile.top_type != null)
			{
				terraformTile(pTile, pTile.main_type, null, AssetManager.terraform.get(pTerraformOption));
			}
			else if (pTile.Type.increase_to != null)
			{
				terraformMain(pTile, pTile.Type.increase_to, AssetManager.terraform.get(pTerraformOption));
			}
		}
	}

	public static void removeLiquid(WorldTile pTile)
	{
		if (pTile.Type.liquid)
		{
			decreaseTile(pTile, pDamage: false);
		}
	}

	public static void growGreens(WorldTile pTile, TopTileType pTopType)
	{
		terraformTop(pTile, pTopType, TerraformLibrary.flash);
	}

	public static void removeGreens(WorldTile pTile)
	{
		decreaseTile(pTile, pDamage: false);
	}

	private static void applyLightningEffect(WorldTile pTile)
	{
		if (pTile.Type.lava && pTile.heat > 20)
		{
			decreaseTile(pTile, pDamage: true);
			if (Randy.randomChance(0.9f))
			{
				int tExtra = pTile.heat / 10;
				World.world.drop_manager.spawnParabolicDrop(pTile, "lava", 0f, 0.15f, 33f + (float)(tExtra * 2), 1f, 40f + (float)tExtra);
			}
			AchievementLibrary.lava_strike.check();
		}
		if (pTile.Type.layer_type == TileLayerType.Ocean)
		{
			removeLiquid(pTile);
			if (Randy.randomChance(0.8f))
			{
				World.world.drop_manager.spawnParabolicDrop(pTile, "rain", 0f, 1f, 66f, 1f, 45f);
			}
		}
		if (pTile.hasBuilding() && pTile.building.asset.spawn_drops)
		{
			if (!pTile.building.data.hasFlag("stop_spawn_drops"))
			{
				pTile.building.spawnBurstSpecial(10);
			}
			if (pTile.building.data.hasFlag("stop_spawn_drops"))
			{
				pTile.building.data.removeFlag("stop_spawn_drops");
			}
			else
			{
				pTile.building.data.addFlag("stop_spawn_drops");
			}
		}
	}

	public static void applyTileDamage(WorldTile pTargetTile, float pRad, TerraformOptions pOptions)
	{
		World.world.resetRedrawTimer();
		BrushData pBrush = Brush.get((int)pRad);
		World.world.conway_layer.checkKillRange(pTargetTile.pos, pBrush.size);
		if (pOptions.remove_tornado)
		{
			tryRemoveTornadoFromTile(pTargetTile);
		}
		WorldBehaviourTileEffects.checkTileForEffectKill(pTargetTile, pBrush.size);
		for (int i = 0; i < pBrush.pos.Length; i++)
		{
			BrushPixelData tBrushPixel = pBrush.pos[i];
			int tX = pTargetTile.pos.x + tBrushPixel.x;
			int tY = pTargetTile.pos.y + tBrushPixel.y;
			if (tX < 0 || tX >= MapBox.width || tY < 0 || tY >= MapBox.height)
			{
				continue;
			}
			WorldTile tTile = World.world.GetTileSimple(tX, tY);
			if (tTile.Type.grey_goo)
			{
				Config.grey_goo_damaged = true;
			}
			if (pOptions.add_burned && !tTile.Type.liquid)
			{
				tTile.setBurned();
			}
			if (pOptions.lightning_effect)
			{
				applyLightningEffect(tTile);
			}
			if (pOptions.add_heat != 0)
			{
				World.world.heat.addTile(tTile, pOptions.add_heat);
			}
			if (tTile.hasBuilding() && pOptions.damage_buildings)
			{
				bool tTryDamageBuilding = true;
				if (pOptions.ignore_kingdoms != null && tTile.building.isAlive() && !tTile.building.kingdom.isNature())
				{
					for (int j = 0; j < pOptions.ignore_kingdoms.Length; j++)
					{
						string tKingdomID = pOptions.ignore_kingdoms[j];
						Kingdom tKingdom = World.world.kingdoms_wild.get(tKingdomID);
						if (tTile.building.kingdom == tKingdom)
						{
							tTryDamageBuilding = false;
						}
					}
				}
				if (tTryDamageBuilding)
				{
					tTile.building.getHit(pOptions.damage);
				}
			}
			if (pOptions.set_fire)
			{
				tTile.startFire(pForce: true);
			}
			bool tTileExploded = false;
			if (pOptions.explode_tile)
			{
				tTileExploded = explodeTile(tTile, tBrushPixel.dist, pRad, pTargetTile, pOptions);
			}
			if (pOptions.transform_to_wasteland && !tTileExploded)
			{
				checkAcidTerraform(tTile);
			}
			if (tTile.hasUnits() && !string.IsNullOrEmpty(pOptions.add_trait))
			{
				tTile.doUnits(delegate(Actor tActor)
				{
					tActor.addTrait(pOptions.add_trait);
				});
			}
		}
	}

	public static bool explodeTile(WorldTile pTile, float pDist, float pRadius, WorldTile pExplosionCenter, TerraformOptions pOptions)
	{
		if (pOptions.damage > 0)
		{
			pTile.doUnits(delegate(Actor tActor)
			{
				if (!tActor.asset.very_high_flyer || pOptions.applies_to_high_flyers)
				{
					tActor.getHit(pOptions.damage, pFlash: true, AttackType.Explosion);
				}
			});
		}
		if (pTile.isTemporaryFrozen())
		{
			pTile.unfreeze();
		}
		float tMode = 0f;
		tMode = pDist / pRadius;
		float tModeInverse = 1f - tMode;
		int tDamage = (int)(30f * tModeInverse);
		if (tDamage <= 0)
		{
			return false;
		}
		bool wasLiquid = pTile.Type.liquid;
		if (!pTile.Type.explodable && Randy.random() > tModeInverse)
		{
			return false;
		}
		World.world.game_stats.data.pixelsExploded++;
		if (pOptions.explosion_pixel_effect)
		{
			World.world.explosion_layer.setDirty(pTile, pDist, pRadius);
		}
		tDamage -= (int)((double)tDamage * 0.5 * (double)Randy.random() * (double)tMode);
		if (pTile.Type.explodable && pTile.explosion_wave == 0)
		{
			World.world.explosion_layer.explodeBomb(pTile);
		}
		if (pTile.Type.explodable_delayed)
		{
			World.world.explosion_layer.activateDelayedBomb(pTile);
		}
		if (pTile.Type.strength <= pOptions.explode_strength)
		{
			decreaseTile(pTile, pDamage: true, TerraformLibrary.flash);
		}
		if (pTile.hasBuilding() && pTile.Type.liquid && !pTile.building.asset.can_be_placed_on_liquid)
		{
			pTile.building.startDestroyBuilding();
		}
		if (!wasLiquid)
		{
			pTile.setBurned();
			if (pOptions.explode_and_set_random_fire)
			{
				if ((double)Randy.random() > 0.8)
				{
					pTile.startFire(pForce: true);
				}
				else
				{
					pTile.startFire();
				}
			}
		}
		return true;
	}

	public static void damageWorld(WorldTile pTile, int pRad, TerraformOptions pOptions, BaseSimObject pByWho = null)
	{
		if (pOptions.shake)
		{
			World.world.startShake(pOptions.shake_duration, pOptions.shake_interval, pOptions.shake_intensity);
		}
		if (pOptions.apply_force)
		{
			World.world.applyForceOnTile(pTile, pRad, pOptions.force_power, pForceOut: true, pOptions.damage, pOptions.ignore_kingdoms, pByWho, pOptions);
		}
		applyTileDamage(pTile, pRad, pOptions);
	}

	public static void makeTileChanged(WorldTile pTile)
	{
		World.world.resetRedrawTimer();
	}

	public static void removeLifeFromTile(WorldTile pTile)
	{
		World.world.conway_layer.remove(pTile);
		if (pTile.Type.life)
		{
			decreaseTile(pTile, pDamage: false, "destroy_life");
		}
		double tClickStartedAt = World.world.player_control.click_started_at;
		pTile.doUnits(delegate(Actor tActor)
		{
			if (tActor.a.asset.can_be_killed_by_life_eraser && !(tActor.a.created_time_unscaled >= tClickStartedAt))
			{
				AchievementLibrary.not_on_my_watch.check(tActor);
				tActor.applyRandomForce();
				tActor.getHitFullHealth(AttackType.Divine);
			}
		});
		HashSet<TornadoEffect> tEffects = TornadoEffect.getTornadoesFromTile(pTile);
		if (tEffects == null)
		{
			return;
		}
		using ListPool<TornadoEffect> tEffectsList = new ListPool<TornadoEffect>(tEffects);
		foreach (ref TornadoEffect item in tEffectsList)
		{
			item.die();
		}
	}

	public static void createRoadTile(WorldTile pTile)
	{
		terraformTop(pTile, TopTileLibrary.road, AssetManager.terraform.get("road"));
	}

	public static void createRoadTilesToBuild(List<WorldTile> pPath, WorldTile pFrom, WorldTile pTarget, bool pForceFinished = false)
	{
		if (pPath.Count > 20 || (pTarget.road_island != null && pTarget.road_island == pFrom.road_island))
		{
			return;
		}
		for (int i = 0; i < pPath.Count; i++)
		{
			WorldTile tTile = pPath[i];
			if (!tTile.Type.road)
			{
				if (pFrom != tTile && pFrom.road_island != null && tTile.road_island == pTarget.road_island)
				{
					return;
				}
				temp_list_tiles_road_tiles_to_build.Add(tTile);
				if (pForceFinished)
				{
					createRoadTile(tTile);
				}
			}
		}
		World.world.resetRedrawTimer();
	}

	public static void makeRoadBetween(WorldTile pTile1, WorldTile pTile2, City pCity = null, bool pForceFinished = false)
	{
		if (pTile1.road_island == null || pTile1.road_island != pTile2.road_island)
		{
			temp_list_tiles_road_path.Clear();
			temp_list_tiles_road_tiles_to_build.Clear();
			World.world.pathfinding_param.resetParam();
			World.world.pathfinding_param.roads = true;
			World.world.calcPath(pTile1, pTile2, temp_list_tiles_road_path);
			createRoadTilesToBuild(temp_list_tiles_road_path, pTile1, pTile2, pForceFinished);
			pCity?.addRoads(temp_list_tiles_road_tiles_to_build);
		}
	}

	public static void tryRemoveTornadoFromTile(WorldTile pTile)
	{
		HashSet<TornadoEffect> tEffects = TornadoEffect.getTornadoesFromTile(pTile);
		if (tEffects == null)
		{
			return;
		}
		using ListPool<TornadoEffect> tEffectsList = new ListPool<TornadoEffect>(tEffects);
		foreach (ref TornadoEffect item in tEffectsList)
		{
			item.die();
		}
	}

	public static void checkSantaHit(Vector2Int pPos, int pRad)
	{
		List<BaseEffect> tList = World.world.stack_effects.get("fx_santa").getList();
		for (int i = 0; i < tList.Count; i++)
		{
			Santa tSanta = tList[i].GetComponent<Santa>();
			if (tSanta.active && tSanta.alive)
			{
				Vector3 tSantaPos = tSanta.transform.localPosition;
				if (!(Toolbox.Dist(pPos.x, 0f, tSantaPos.x, 0f) > (float)pRad) && !(tSantaPos.y < (float)pPos.y) && !(tSantaPos.y - 20f > (float)pPos.y))
				{
					tSanta.alive = false;
					AchievementLibrary.mayday.check();
				}
			}
		}
	}

	public static void checkUFOHit(Vector2Int pPos, int pRad, Actor pActor)
	{
		Kingdom ufoKingdom = World.world.kingdoms_wild.get("aliens");
		if (ufoKingdom.units.Count == 0)
		{
			return;
		}
		List<Actor> list = ufoKingdom.units;
		for (int i = 0; i < list.Count; i++)
		{
			Actor tObject = list[i];
			if (tObject.isAlive())
			{
				Vector3 tCurrentPos = tObject.current_position;
				if (!(Toolbox.Dist(pPos.x, 0f, tCurrentPos.x, 0f) > (float)pRad) && !(tCurrentPos.y < (float)pPos.y) && !(tCurrentPos.y - 10f > (float)pPos.y) && tObject.asset.flag_ufo)
				{
					tObject.getHit(tObject.getHealth(), pFlash: true, AttackType.Other, pActor);
				}
			}
		}
	}

	public static void checkTornadoHit(Vector2Int pPos, int pRad)
	{
		if (!World.world.stack_effects.get("fx_tornado").isAnyActive())
		{
			return;
		}
		using ListPool<BaseEffect> tList = new ListPool<BaseEffect>(World.world.stack_effects.get("fx_tornado").getList());
		for (int i = 0; i < tList.Count; i++)
		{
			if (tList[i].active)
			{
				TornadoEffect tTornado = (TornadoEffect)tList[i];
				if (!tTornado.isKilled() && !(Toolbox.DistVec2Float(tTornado.transform.localPosition, pPos) > (float)pRad))
				{
					tTornado.split();
				}
			}
		}
	}

	public static void checkLightningAction(Vector2Int pPos, int pRad, Actor pActor = null, bool pCheckForImmortal = false, bool pCheckMayIInterrupt = false)
	{
		bool tImmortalGiven = false;
		int tRad = pRad * pRad;
		List<Actor> tList = World.world.units.getSimpleList();
		for (int i = 0; i < tList.Count; i++)
		{
			Actor tActor = tList[i];
			if (Toolbox.SquaredDistVec2(tActor.current_tile.pos, pPos) > tRad)
			{
				continue;
			}
			if (tActor.asset.flag_finger)
			{
				tActor.getActorComponent<GodFinger>().lightAction();
				tActor.getHit(1f, pFlash: true, AttackType.Other, pActor);
				continue;
			}
			if (pCheckForImmortal && !tImmortalGiven && !tActor.hasTrait("immortal") && Randy.randomChance(0.2f))
			{
				tActor.addTrait("immortal");
				tActor.addTrait("energized");
				tImmortalGiven = true;
			}
			if (pCheckMayIInterrupt)
			{
				AchievementLibrary.may_i_interrupt.checkBySignal(tActor.ai.task?.id);
			}
		}
	}
}
