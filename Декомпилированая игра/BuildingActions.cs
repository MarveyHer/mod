public static class BuildingActions
{
	public static void tryGrowVegetationRandom(WorldTile pTile, VegetationType pType, bool pOnStart = false, bool pCheckLimit = true, bool pCheckRandom = true)
	{
		BiomeAsset tBiome = pTile.Type.biome_asset;
		if (tBiome == null || !tBiome.grow_vegetation_auto)
		{
			return;
		}
		BuildingAsset tAsset = null;
		switch (pType)
		{
		case VegetationType.Plants:
			if (tBiome.grow_type_selector_plants != null)
			{
				tAsset = tBiome.grow_type_selector_plants(pTile);
			}
			break;
		case VegetationType.Trees:
			if (tBiome.grow_type_selector_trees != null)
			{
				tAsset = tBiome.grow_type_selector_trees(pTile);
			}
			break;
		case VegetationType.Bushes:
			if (tBiome.grow_type_selector_bushes != null)
			{
				tAsset = tBiome.grow_type_selector_bushes(pTile);
			}
			break;
		}
		if (tAsset == null)
		{
			return;
		}
		if (tAsset.limit_in_radius > 0)
		{
			pCheckLimit = true;
		}
		if ((!pCheckLimit || !pTile.zone.hasReachedBuildingLimit(pTile, tAsset)) && (!pCheckRandom || !(tAsset.vegetation_random_chance < Randy.random())) && World.world.buildings.canBuildFrom(pTile, tAsset, null))
		{
			World.world.buildings.addBuilding(tAsset, pTile);
			if (tAsset.flora_type == FloraType.Tree)
			{
				World.world.game_stats.data.treesGrown++;
			}
			else if (tAsset.flora_type == FloraType.Plant || tAsset.flora_type == FloraType.Fungi)
			{
				World.world.game_stats.data.floraGrown++;
			}
			if (tAsset.has_sound_spawn)
			{
				MusicBox.playSound(tAsset.sound_spawn, pTile, pGameViewOnly: true, pVisibleOnly: true);
			}
		}
	}

	public static void tryGrowMineralRandom(WorldTile pTile, bool pOnStart = false, bool pCheckLimit = true)
	{
		BiomeAsset tBiome = pTile.getBiome();
		if (tBiome != null && tBiome.grow_minerals_auto && (!pTile.hasBuilding() || !pTile.building.isUsable()))
		{
			BuildingAsset tTempAsset = tBiome.grow_type_selector_minerals(pTile);
			if (tTempAsset != null && (!pCheckLimit || !pTile.zone.hasReachedBuildingLimit(pTile, tTempAsset)) && World.world.buildings.canBuildFrom(pTile, tTempAsset, null))
			{
				World.world.buildings.addBuilding(tTempAsset, pTile);
			}
		}
	}

	public static Building tryGrowVegetation(WorldTile pTile, string pTemplateID, bool pSfx = false, bool pCheckLimit = true)
	{
		BuildingAsset tTempPlant = AssetManager.buildings.get(pTemplateID);
		if (pTile.hasBuilding() && pTile.building.isUsable())
		{
			return null;
		}
		if (tTempPlant == null)
		{
			return null;
		}
		if (pCheckLimit && pTile.zone.hasReachedBuildingLimit(pTile, tTempPlant))
		{
			return null;
		}
		if (!World.world.buildings.canBuildFrom(pTile, tTempPlant, null))
		{
			return null;
		}
		Building result = World.world.buildings.addBuilding(tTempPlant, pTile, pCheckForBuild: false, pSfx);
		World.world.game_stats.data.floraGrown++;
		return result;
	}

	public static void spawnBeehives(int pAmount)
	{
		for (int i = 0; i < pAmount; i++)
		{
			WorldTile tTile = World.world.tiles_list.GetRandom();
			if (tTile.Type.grass)
			{
				World.world.buildings.addBuilding("beehive", tTile, pCheckForBuild: true);
			}
		}
	}

	public static void spawnResource(int pAmount, string pType, bool pRandomSize = true)
	{
		for (int i = 0; i < pAmount; i++)
		{
			WorldTile tTile = World.world.tiles_list.GetRandom();
			if (tTile.Type.ground)
			{
				World.world.buildings.addBuilding(pType, tTile, pCheckForBuild: true);
			}
		}
	}
}
