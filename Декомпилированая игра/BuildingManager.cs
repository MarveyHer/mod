using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tools;
using UnityEngine;

public class BuildingManager : SimSystemManager<Building, BuildingData>
{
	private List<WorldTile> _temp_list_tiles = new List<WorldTile>();

	private JobManagerBuildings _job_manager;

	private Building[] _array_visible_buildings = new Building[0];

	private int _visible_buildings_count;

	public BuildingRenderData render_data = new BuildingRenderData(4096);

	public HashSet<Building> occupied_buildings = new HashSet<Building>();

	public List<Building> visible_stockpiles = new List<Building>();

	public List<Building> sparkles = new List<Building>();

	public MultiStackPool<BaseBuildingComponent> component_pool = new MultiStackPool<BaseBuildingComponent>();

	private bool _need_normal_check;

	public BuildingManager()
	{
		type_id = "building";
		_job_manager = new JobManagerBuildings("buildings");
	}

	public override void clear()
	{
		_job_manager.clear();
		Array.Clear(_array_visible_buildings, 0, _array_visible_buildings.Length);
		_temp_list_tiles.Clear();
		occupied_buildings.Clear();
		checkContainer();
		scheduleDestroyAllOnWorldClear();
		checkObjectsToDestroy();
		base.clear();
	}

	protected override void destroyObject(Building pBuilding)
	{
		base.destroyObject(pBuilding);
		if (pBuilding.hasHousingLogic())
		{
			event_houses = true;
		}
		pBuilding.setAlive(pValue: false);
		pBuilding.asset.buildings.Remove(pBuilding);
		occupied_buildings.Remove(pBuilding);
		removeObject(pBuilding);
		_job_manager.removeObject(pBuilding, pBuilding.batch);
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		Bench.bench("buildings", "game_total");
		checkContainer();
		_job_manager.updateBase(pElapsed);
		checkContainer();
		Bench.benchEnd("buildings", "game_total", pSaveCounter: false, 0L);
	}

	public override void loadFromSave(List<BuildingData> pList)
	{
		base.loadFromSave(pList);
		checkContainer();
	}

	internal Building addBuilding(string pID, WorldTile pTile, bool pCheckForBuild = false, bool pSfx = false, BuildPlacingType pType = BuildPlacingType.New)
	{
		BuildingAsset tAsset = AssetManager.buildings.get(pID);
		return addBuilding(tAsset, pTile, pCheckForBuild, pSfx, pType);
	}

	internal Building addBuilding(BuildingAsset pAsset, WorldTile pTile, bool pCheckForBuild = false, bool pSfx = false, BuildPlacingType pType = BuildPlacingType.New)
	{
		if (pCheckForBuild && !canBuildFrom(pTile, pAsset, null, pType))
		{
			return null;
		}
		Building building = newObject();
		building.create();
		building.setBuilding(pTile, pAsset, null);
		building.checkStartSpawnAnimation();
		if (building.asset.city_building)
		{
			World.world.map_stats.housesBuilt++;
		}
		return building;
	}

	protected override void addObject(Building pObject)
	{
		base.addObject(pObject);
		_job_manager.addNewObject(pObject);
	}

	public override Building loadObject(BuildingData pData)
	{
		if (pData.state == BuildingState.Removed)
		{
			return null;
		}
		BuildingAsset tAsset = AssetManager.buildings.get(pData.asset_id);
		if (tAsset == null)
		{
			return null;
		}
		WorldTile tTile = World.world.GetTileSimple(pData.mainX, pData.mainY);
		if (!canBuildFrom(tTile, tAsset, null, BuildPlacingType.Load))
		{
			return null;
		}
		Building building = base.loadObject(pData);
		building.create();
		building.setBuilding(tTile, tAsset, pData);
		building.loadBuilding(pData);
		return building;
	}

	internal bool canBuildFrom(WorldTile pTile, BuildingAsset pNewBuildingAsset, City pCity, BuildPlacingType pType = BuildPlacingType.New, bool pFloraGrowth = false)
	{
		Subspecies tSubspecies = pCity?.getMainSubspecies();
		bool tCheckForAdaptation = tSubspecies != null && pNewBuildingAsset.city_building && pNewBuildingAsset.check_for_adaptation_tags;
		if (tCheckForAdaptation && pTile.Type.is_biome)
		{
			string tBuildTag = pTile.Type.only_allowed_to_build_with_tag;
			if (tBuildTag != null && !tSubspecies.hasMetaTag(tBuildTag))
			{
				return false;
			}
		}
		BuildingFundament tFundament = pNewBuildingAsset.fundament;
		int tCenterX = pTile.x - tFundament.left;
		int tCenterY = pTile.y - tFundament.bottom;
		int tWidth = tFundament.width;
		int tHeight = tFundament.height;
		bool tGroundFound = false;
		bool tWaterFound = false;
		bool tIsDock = pNewBuildingAsset.docks;
		List<WorldTile> tTempList = _temp_list_tiles;
		tTempList.Clear();
		bool tCheckForTags = !WorldLawLibrary.world_law_roots_without_borders.isEnabled();
		WorldTile tCityTile = pCity?.getTile();
		if (pCity != null && tCityTile == null)
		{
			return false;
		}
		bool tTinyOvergrow = pType == BuildPlacingType.New && Randy.randomChance(0.1f);
		for (int tX = 0; tX < tWidth; tX++)
		{
			for (int tY = 0; tY < tHeight; tY++)
			{
				WorldTile tTile = World.world.GetTile(tCenterX + tX, tCenterY + tY);
				if (tTile == null)
				{
					return false;
				}
				if (tCheckForAdaptation)
				{
					string tBuildTag2 = tTile.Type.only_allowed_to_build_with_tag;
					if (tBuildTag2 != null && !tSubspecies.hasMetaTag(tBuildTag2))
					{
						return false;
					}
				}
				tTempList.Add(tTile);
				Building tCurrentBuildingOnTile = tTile.building;
				TileTypeBase tTileType = tTile.Type;
				if (tIsDock)
				{
					if (tTileType.ocean && OceanHelper.goodForNewDock(tTile))
					{
						tWaterFound = true;
					}
					if (tTileType.ground)
					{
						tGroundFound = true;
					}
				}
				if (pCity != null)
				{
					if (!tIsDock && !tTile.isSameIsland(tCityTile))
					{
						return false;
					}
					if (!tTile.isSameCityHere(pCity))
					{
						return false;
					}
					if (pNewBuildingAsset.only_build_tiles && !tTileType.can_build_on)
					{
						return false;
					}
				}
				if ((pType != BuildPlacingType.Load || (!(tTileType.id == "frozen_low") && !(tTileType.id == "frozen_high"))) && tCheckForTags && !pNewBuildingAsset.isOverlaysBiomeTags(tTileType))
				{
					if (!pFloraGrowth)
					{
						return false;
					}
					if (!pNewBuildingAsset.isOverlaysBiomeSpreadTags(tTileType))
					{
						return false;
					}
				}
				if (pNewBuildingAsset.flora && tCurrentBuildingOnTile != null)
				{
					if (!tCurrentBuildingOnTile.asset.flora)
					{
						return false;
					}
					if (pNewBuildingAsset.flora_size <= tCurrentBuildingOnTile.asset.flora_size)
					{
						if (tTinyOvergrow && tCurrentBuildingOnTile.asset.flora_size == FloraSize.Tiny && tCurrentBuildingOnTile.asset.flora_size == pNewBuildingAsset.flora_size)
						{
							if (tCurrentBuildingOnTile.asset == pNewBuildingAsset)
							{
								return false;
							}
						}
						else if (!tCurrentBuildingOnTile.isRuin())
						{
							return false;
						}
					}
					if (!tTile.canGrow())
					{
						return false;
					}
				}
				if (tTileType.liquid && !pNewBuildingAsset.can_be_placed_on_liquid)
				{
					return false;
				}
				if (pNewBuildingAsset.destroy_on_liquid && tTileType.ocean)
				{
					return false;
				}
				if (!tTile.canBuildOn(pNewBuildingAsset))
				{
					return false;
				}
				if (!pNewBuildingAsset.check_for_close_building || pType != BuildPlacingType.New)
				{
					continue;
				}
				if (tX == 0)
				{
					if (isBuildingNearby(tTile.tile_left))
					{
						return false;
					}
				}
				else if (tX == tWidth - 1 && isBuildingNearby(tTile.tile_right))
				{
					return false;
				}
				if (tY == 0)
				{
					if (isBuildingNearby(tTile.tile_down))
					{
						return false;
					}
					if (tTile.has_tile_down && isBuildingNearby(tTile.tile_down.tile_down))
					{
						return false;
					}
				}
				else if (tY == tHeight - 1)
				{
					if (isBuildingNearby(tTile.tile_up))
					{
						return false;
					}
					if (tTile.has_tile_up && isBuildingNearby(tTile.tile_up.tile_up))
					{
						return false;
					}
				}
			}
		}
		if (tIsDock && pType == BuildPlacingType.New)
		{
			if (tWaterFound && !tGroundFound)
			{
				for (int i = 0; i < tTempList.Count; i++)
				{
					WorldTile tDockTile = tTempList[i];
					for (int j = 0; j < tDockTile.neighbours.Length; j++)
					{
						WorldTile tN = tDockTile.neighbours[j];
						if (tN.Type.ground && tN.region.island == tCityTile?.region.island)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return true;
	}

	private bool isBuildingNearby(WorldTile pTile)
	{
		if (pTile == null)
		{
			return true;
		}
		Building tBuilding = pTile.building;
		if (tBuilding != null && tBuilding.isUsable() && tBuilding.asset.city_building)
		{
			return true;
		}
		return false;
	}

	public Building getNearbyBuildingToLive(Actor pActor, bool pOnlyBuilt)
	{
		foreach (Building tTarget in getBuildingFromZones(pActor.current_tile, 10f))
		{
			if (!tTarget.asset.hasHousingSlots() || !tTarget.current_tile.isSameIsland(pActor.current_tile) || !tTarget.hasResidentSlots())
			{
				continue;
			}
			if (pOnlyBuilt)
			{
				if (tTarget.isUnderConstruction())
				{
					continue;
				}
			}
			else if (!tTarget.isUnderConstruction())
			{
				continue;
			}
			if (tTarget.kingdom == pActor.kingdom)
			{
				return tTarget;
			}
		}
		return null;
	}

	public IEnumerable<Building> getBuildingFromZones(WorldTile pTile, float pRadius)
	{
		foreach (Building item in checkZoneForBuilding(pTile, pTile.zone, pRadius))
		{
			yield return item;
		}
		float tRadiusZones = pRadius / 8f;
		int tSize = (int)tRadiusZones + 1;
		int startX = pTile.zone.x - tSize;
		int startY = pTile.zone.y - tSize;
		for (int iX = 0; iX < tSize * 2; iX++)
		{
			for (int iY = 0; iY < tSize * 2; iY++)
			{
				TileZone tZone = World.world.zone_calculator.getZone(iX + startX, iY + startY);
				if (tZone == null)
				{
					continue;
				}
				foreach (Building item2 in checkZoneForBuilding(pTile, tZone, pRadius))
				{
					yield return item2;
				}
			}
		}
	}

	private IEnumerable<Building> checkZoneForBuilding(WorldTile pTile, TileZone pZone, float pRadius)
	{
		if (!pZone.buildings_all.Any())
		{
			yield break;
		}
		float tRadius = pRadius * pRadius;
		foreach (Building tBuilding in pZone.buildings_all)
		{
			if ((tRadius == 0f || !((float)Toolbox.SquaredDistTile(tBuilding.current_tile, pTile) > tRadius)) && !tBuilding.isRuin() && tBuilding.current_tile.isSameIsland(pTile))
			{
				yield return tBuilding;
			}
		}
	}

	public void debugJobManager(DebugTool pTool)
	{
		_job_manager.debug(pTool);
	}

	private void prepareLists()
	{
		_array_visible_buildings = Toolbox.checkArraySize(_array_visible_buildings, Count);
		render_data.checkSize(Count);
		visible_stockpiles.Clear();
		sparkles.Clear();
		checkContainer();
	}

	internal void calculateVisibleBuildings()
	{
		Bench.bench("buildings_prepare", "game_total");
		prepareLists();
		_visible_buildings_count = 0;
		Bench.benchEnd("buildings_prepare", "game_total", pSaveCounter: false, 0L);
		if (!World.world.quality_changer.shouldRenderBuildings())
		{
			Bench.clearBenchmarkEntrySkipMultiple("game_total", "buildings_render_data_parallel_256", "buildings_fill_visible", "buildings_render_data_normal");
			return;
		}
		Bench.bench("buildings_fill_visible", "game_total");
		fillVisibleObjects();
		Bench.benchEnd("buildings_fill_visible", "game_total", pSaveCounter: false, 0L);
		Bench.bench("buildings_render_data_parallel_256", "game_total");
		precalculateRenderDataParallel();
		Bench.benchEnd("buildings_render_data_parallel_256", "game_total", pSaveCounter: false, 0L);
		Bench.bench("buildings_render_data_normal", "game_total");
		precalculateRenderDataNormal();
		Bench.benchEnd("buildings_render_data_normal", "game_total", pSaveCounter: false, 0L);
	}

	private void fillVisibleObjects()
	{
		Building[] tArrayVisibleBuildings = _array_visible_buildings;
		List<TileZone> tZonesList = World.world.zone_camera.getVisibleZones();
		int tZonesCount = tZonesList.Count;
		int tTotalVisibleBuildings = 0;
		for (int iZone = 0; iZone < tZonesCount; iZone++)
		{
			List<Building> buildings_render_list = tZonesList[iZone].buildings_render_list;
			int tZoneBuildings = buildings_render_list.Count;
			buildings_render_list.CopyTo(tArrayVisibleBuildings, tTotalVisibleBuildings);
			tTotalVisibleBuildings += tZoneBuildings;
		}
		_visible_buildings_count = tTotalVisibleBuildings;
	}

	private void precalculateRenderDataParallel()
	{
		Building[] tArrayVisibleBuildings = _array_visible_buildings;
		bool tNeedShadows = World.world.quality_changer.shouldRenderBuildingShadows();
		int tTotalVisibleObjects = _visible_buildings_count;
		Vector3[] tRenderScales = render_data.scales;
		Vector3[] tRenderPositions = render_data.positions;
		Vector3[] tRenderRotations = render_data.rotations;
		Material[] tRenderMaterials = render_data.materials;
		bool[] tRenderFlipXStates = render_data.flip_x_states;
		Color[] tRenderColors = render_data.colors;
		Sprite[] tRenderMainSprites = render_data.main_sprites;
		Sprite[] tRenderColoredSprites = render_data.colored_sprites;
		bool[] tRenderShadows = render_data.shadows;
		Sprite[] tRenderShadowSprites = render_data.shadow_sprites;
		int tDynamicBatchSize = 256;
		int tTotalBatches = ParallelHelper.calcTotalBatches(tTotalVisibleObjects, tDynamicBatchSize);
		bool tNeedNormalCheck = false;
		Parallel.For(0, tTotalBatches, World.world.parallel_options, delegate(int pBatchIndex)
		{
			int num = ParallelHelper.calculateBatchBeg(pBatchIndex, tDynamicBatchSize);
			int num2 = ParallelHelper.calculateBatchEnd(num, tDynamicBatchSize, tTotalVisibleObjects);
			for (int i = num; i < num2; i++)
			{
				Building building = tArrayVisibleBuildings[i];
				BuildingAsset asset = building.asset;
				tRenderScales[i] = building.getCurrentScale();
				tRenderPositions[i] = building.cur_transform_position;
				tRenderRotations[i] = building.current_rotation;
				tRenderMaterials[i] = building.material;
				tRenderFlipXStates[i] = building.flip_x;
				tRenderColors[i] = building.kingdom.asset.color_building;
				Sprite sprite = building.calculateMainSprite();
				tRenderMainSprites[i] = sprite;
				if (building.isColoredSpriteNeedsCheck(sprite))
				{
					tRenderColoredSprites[i] = null;
					tNeedNormalCheck = true;
				}
				else
				{
					tRenderColoredSprites[i] = building.getLastColoredSprite();
				}
				if (tNeedShadows)
				{
					tRenderShadows[i] = asset.shadow && !building.chopped;
					tRenderShadowSprites[i] = DynamicSprites.getShadowBuilding(building.asset, tRenderMainSprites[i]);
				}
				if (asset.is_stockpile)
				{
					tNeedNormalCheck = true;
				}
				if (asset.sparkle_effect)
				{
					tNeedNormalCheck = true;
				}
			}
		});
		_need_normal_check = tNeedNormalCheck;
	}

	private void precalculateRenderDataNormal()
	{
		if (!_need_normal_check)
		{
			return;
		}
		BuildingRenderData buildingRenderData = render_data;
		int tTotalVisibleBuildings = _visible_buildings_count;
		Sprite[] tRenderColoredSprites = buildingRenderData.colored_sprites;
		Sprite[] tRenderMainSprites = buildingRenderData.main_sprites;
		for (int i = 0; i < tTotalVisibleBuildings; i++)
		{
			Building tBuilding = _array_visible_buildings[i];
			if (tBuilding.asset.is_stockpile)
			{
				visible_stockpiles.Add(tBuilding);
			}
			if (tBuilding.asset.sparkle_effect)
			{
				sparkles.Add(tBuilding);
			}
			if ((object)tRenderColoredSprites[i] == null)
			{
				tRenderColoredSprites[i] = tBuilding.calculateColoredSprite(tRenderMainSprites[i]);
			}
		}
	}

	public Building[] getVisibleBuildings()
	{
		return _array_visible_buildings;
	}

	public int countVisibleBuildings()
	{
		return _visible_buildings_count;
	}

	public void checkWobblySetting()
	{
		bool tWobblySettingsOn = PlayerConfig.optionEnabled("tree_wind", OptionType.Bool);
		foreach (DynamicSpritesAsset tAtlasAsset in AssetManager.dynamic_sprites_library.list)
		{
			if (tAtlasAsset.check_wobbly_setting)
			{
				tAtlasAsset.big_atlas = !tWobblySettingsOn;
			}
		}
		foreach (DynamicSpritesAsset tAtlasAsset2 in AssetManager.dynamic_sprites_library.list)
		{
			if (tAtlasAsset2.buildings)
			{
				tAtlasAsset2.resetAtlas();
			}
		}
		AssetManager.buildings.checkAtlasLink(tWobblySettingsOn);
		using IEnumerator<Building> enumerator2 = GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Building current = enumerator2.Current;
			current.checkMaterial();
			current.clearSprites();
		}
	}

	public JobManagerBuildings getJobManager()
	{
		return _job_manager;
	}
}
