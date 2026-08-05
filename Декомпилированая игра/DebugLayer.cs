using System.Collections.Generic;
using UnityEngine;

public class DebugLayer : MapLayer
{
	internal static List<TileZone> fmod_zones_to_draw = new List<TileZone>();

	private HashSet<WorldTile> _tiles = new HashSet<WorldTile>();

	public Color color1 = Color.gray;

	public Color color2 = Color.white;

	public Color color_red = Color.red;

	public Color color_active_path;

	private bool used;

	private List<MapRegion> _forced_global_path = new List<MapRegion>();

	protected override void UpdateDirty(float pElapsed)
	{
		if (!DebugConfig.instance.debugButton.gameObject.activeSelf)
		{
			clear();
			return;
		}
		color_active_path = new Color(1f, 1f, 1f, 0.5f);
		used = false;
		clear();
		if (_forced_global_path != null && _forced_global_path.Count > 0)
		{
			drawRegionPath(_forced_global_path);
		}
		if (DebugConfig.isOn(DebugOption.CityZones))
		{
			drawZones();
		}
		else if (DebugConfig.isOn(DebugOption.Chunks))
		{
			drawChunks();
		}
		if (DebugConfig.isOn(DebugOption.PathRegions))
		{
			drawPathRegions();
		}
		if (DebugConfig.isOn(DebugOption.ActivePaths))
		{
			drawActivePaths();
		}
		if (DebugConfig.isOn(DebugOption.CityPlaces))
		{
			drawCityPlaces();
		}
		if (DebugConfig.isOn(DebugOption.RenderCityDangerZones))
		{
			drawCityDangerZones();
		}
		if (DebugConfig.isOn(DebugOption.RenderVisibleZones))
		{
			drawVisibleZones();
		}
		if (DebugConfig.isOn(DebugOption.RenderCityCenterZones))
		{
			drawCityCenterZones();
		}
		if (DebugConfig.isOn(DebugOption.RenderCityFarmPlaces))
		{
			drawCityFarmZones();
		}
		if (DebugConfig.isOn(DebugOption.Buildings))
		{
			drawBuildings();
		}
		if (DebugConfig.isOn(DebugOption.FmodZones))
		{
			drawFmodZones();
		}
		if (DebugConfig.isOn(DebugOption.ConstructionTiles))
		{
			drawConstructionTiles();
		}
		if (DebugConfig.isOn(DebugOption.UnitsInside))
		{
			drawUnitsInside();
		}
		if (DebugConfig.isOn(DebugOption.TargetedBy))
		{
			drawTargetedBy();
		}
		if (DebugConfig.isOn(DebugOption.UnitKingdoms))
		{
			drawUnitKingdoms();
		}
		if (DebugConfig.isOn(DebugOption.DisplayUnitTiles))
		{
			drawUnitTiles();
		}
		if (DebugConfig.isOn(DebugOption.ProKing))
		{
			drawProfession(UnitProfession.King);
		}
		if (DebugConfig.isOn(DebugOption.ProLeader))
		{
			drawProfession(UnitProfession.Leader);
		}
		if (DebugConfig.isOn(DebugOption.ProUnit))
		{
			drawProfession(UnitProfession.Unit);
		}
		if (DebugConfig.isOn(DebugOption.ProWarrior))
		{
			drawProfession(UnitProfession.Warrior);
		}
		if (used)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			updatePixels();
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void drawUnitKingdoms()
	{
		used = true;
		foreach (Actor tActor in World.world.units)
		{
			if (tActor.kingdom != null && tActor.kingdom.getColor() != null)
			{
				Color tColor = tActor.kingdom.getColor().getColorMain32();
				pixels[tActor.current_tile.data.tile_id] = tColor;
				_tiles.Add(tActor.current_tile);
			}
		}
	}

	private void drawUnitTiles()
	{
		used = true;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.hasUnits())
			{
				pixels[tTile.data.tile_id] = Color.blue;
				_tiles.Add(tTile);
			}
		}
	}

	private void drawTargetedBy()
	{
		used = true;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.isTargeted())
			{
				pixels[tTile.data.tile_id] = Color.blue;
				_tiles.Add(tTile);
			}
		}
	}

	private void drawProfession(UnitProfession pPro)
	{
		used = true;
		foreach (Actor tActor in World.world.units)
		{
			if (tActor.isProfession(pPro))
			{
				Color tColor = Color.blue;
				pixels[tActor.current_tile.data.tile_id] = tColor;
				_tiles.Add(tActor.current_tile);
			}
		}
	}

	private void drawCitizenJobs(string pID)
	{
		used = true;
		foreach (Actor tActor in World.world.units)
		{
			if (tActor.ai.job != null && !(pID != tActor.ai.job.id))
			{
				Color tColor = Color.red;
				pixels[tActor.current_tile.data.tile_id] = tColor;
				_tiles.Add(tActor.current_tile);
			}
		}
	}

	private void drawUnitsInside()
	{
		used = true;
		foreach (Actor tActor in World.world.units)
		{
			if (tActor.is_inside_building)
			{
				pixels[tActor.current_tile.data.tile_id] = Color.green;
				_tiles.Add(tActor.current_tile);
			}
		}
	}

	private void drawConstructionTiles()
	{
		used = true;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (!tTile.hasBuilding() || !tTile.building.asset.docks)
			{
				continue;
			}
			(TileZone[], int) allZonesFromTile = Toolbox.getAllZonesFromTile(tTile);
			TileZone[] tZones = allZonesFromTile.Item1;
			int tCount = allZonesFromTile.Item2;
			for (int j = 0; j < tCount; j++)
			{
				TileZone tTileZone = tZones[j];
				foreach (WorldTile iTile in tTile.building.checkZoneForDockConstruction(tTileZone))
				{
					pixels[iTile.data.tile_id] = Color.red;
					_tiles.Add(iTile);
				}
			}
		}
	}

	private void drawFmodZones()
	{
		used = true;
		foreach (TileZone tZone in fmod_zones_to_draw)
		{
			fill(tZone.tiles, Color.yellow);
		}
	}

	private void drawBuildings()
	{
		used = true;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.hasBuilding())
			{
				if (tTile.building.kingdom != null && tTile.building.isKingdomCiv())
				{
					pixels[tTile.data.tile_id] = tTile.building.kingdom.getColor().getColorMain32();
				}
				else
				{
					pixels[tTile.data.tile_id] = Color.red;
				}
				pixels[tTile.building.current_tile.data.tile_id] = Color.magenta;
				pixels[tTile.building.door_tile.data.tile_id] = Color.yellow;
				_tiles.Add(tTile.building.current_tile);
				_tiles.Add(tTile.building.door_tile);
				_tiles.Add(tTile);
			}
		}
	}

	private void drawCityCenterZones()
	{
		used = true;
		foreach (City city in World.world.cities)
		{
			WorldTile tTile = city.getTile();
			if (tTile != null)
			{
				fill(tTile.zone.tiles, Color.red);
			}
		}
	}

	private void drawCityFarmZones()
	{
		used = true;
		foreach (City tCity in World.world.cities)
		{
			fill(tCity.calculated_place_for_farms.getSimpleList(), Color.blue);
			fill(tCity.calculated_farm_fields.getSimpleList(), Color.cyan);
			fill(tCity.calculated_crops.getSimpleList(), Color.green);
			fill(tCity.calculated_grown_wheat.getSimpleList(), Color.yellow);
		}
	}

	private void drawVisibleZones()
	{
		used = true;
		List<TileZone> tVisibleZones = World.world.zone_camera.getVisibleZones();
		for (int iZone = 0; iZone < tVisibleZones.Count; iZone++)
		{
			TileZone tZone = tVisibleZones[iZone];
			if (tZone.visible_main_centered)
			{
				fill(tZone.tiles, Color.green);
			}
			else if (tZone.visible)
			{
				fill(tZone.tiles, Color.blue);
			}
		}
	}

	private void drawCityDangerZones()
	{
		used = true;
		foreach (City city in World.world.cities)
		{
			foreach (TileZone tZone in city.danger_zones)
			{
				fill(tZone.tiles, Color.red);
			}
		}
	}

	private void drawCityPlaces()
	{
		used = true;
		foreach (TileZone tZone in World.world.zone_calculator.zones)
		{
			if (tZone.city != null)
			{
				fill(tZone.tiles, Color.yellow);
			}
			else if (tZone.isGoodForNewCity())
			{
				fill(tZone.tiles, Color.blue);
			}
		}
	}

	private void drawActivePaths()
	{
		used = true;
		foreach (Actor tActor in World.world.units)
		{
			if (tActor.current_path_global != null)
			{
				drawRegionPath(tActor.current_path_global);
				fill(tActor.current_path, Color.blue);
			}
		}
	}

	public void drawRegionPath(List<MapRegion> pRegions)
	{
		used = true;
		foreach (MapRegion tRegion in pRegions)
		{
			fill(tRegion.tiles, color_active_path);
		}
	}

	public void forceDrawRegionPath(List<MapRegion> pRegions)
	{
		_forced_global_path.Clear();
		_forced_global_path.AddRange(pRegions);
	}

	private void drawPathRegions()
	{
		used = true;
		MapChunk[] chunks = World.world.map_chunk_manager.chunks;
		for (int i = 0; i < chunks.Length; i++)
		{
			foreach (MapRegion tRegion in chunks[i].regions)
			{
				if (tRegion.path_wave_id != -1)
				{
					fill(tRegion.tiles, new Color(1f, 1f, 0f, 0.9f));
				}
			}
		}
		List<MapRegion> last_globalPath = World.world.region_path_finder.last_globalPath;
		if (last_globalPath == null || last_globalPath.Count <= 0 || World.world.region_path_finder?.tileStart?.region == null || World.world.region_path_finder?.tileTarget?.region == null)
		{
			return;
		}
		foreach (MapRegion tRegion2 in World.world.region_path_finder.last_globalPath)
		{
			fill(tRegion2.tiles, Color.blue);
		}
		fill(World.world.region_path_finder.tileStart.region.tiles, Color.green);
		fill(World.world.region_path_finder.tileTarget.region.tiles, new Color(1f, 0f, 0f, 0.3f));
	}

	private void fill(List<WorldTile> pTiles, Color pColor, bool pEdge = false)
	{
		createTextureNew();
		for (int i = 0; i < pTiles.Count; i++)
		{
			WorldTile tTile = pTiles[i];
			if (!pEdge || tTile.region != null)
			{
				_tiles.Add(tTile);
				pixels[tTile.data.tile_id] = pColor;
			}
		}
	}

	private void fill(WorldTile[] pTiles, Color pColor, bool pEdge = false)
	{
		createTextureNew();
		foreach (WorldTile tTile in pTiles)
		{
			if (!pEdge || tTile.region != null)
			{
				_tiles.Add(tTile);
				pixels[tTile.data.tile_id] = pColor;
			}
		}
	}

	private void drawZones()
	{
		used = true;
		foreach (TileZone tZone in World.world.zone_calculator.zones)
		{
			if ((tZone.x + tZone.y) % 2 == 0)
			{
				tZone.debug_zone_color = color1;
			}
			else
			{
				tZone.debug_zone_color = color2;
			}
			fill(tZone.tiles, tZone.debug_zone_color);
		}
	}

	private void testCityLayout()
	{
		DebugVariables instance = DebugVariables.instance;
		if ((object)instance != null && !instance.layout_city_test)
		{
			return;
		}
		used = true;
		WorldTile tCursorTile = World.world.getMouseTilePos();
		if (tCursorTile == null)
		{
			return;
		}
		TileZone tCursorZone = tCursorTile?.zone;
		foreach (TileZone tZone in World.world.zone_calculator.zones)
		{
			bool tAllow = true;
			if (!TownPlans.debugVisualizeZone(tZone, tCursorZone))
			{
				tAllow = false;
			}
			if (tAllow)
			{
				tZone.debug_zone_color = color1;
			}
			else
			{
				tZone.debug_zone_color = color_red;
			}
			fill(tZone.tiles, tZone.debug_zone_color);
		}
	}

	private void drawChunks()
	{
		used = true;
		MapChunk[] tChunks = World.world.map_chunk_manager.chunks;
		foreach (MapChunk tChunk in tChunks)
		{
			fill(tChunk.tiles, tChunk.color);
		}
	}

	internal override void clear()
	{
		HashSet<WorldTile> tTiles = _tiles;
		if (tTiles.Count == 0)
		{
			return;
		}
		foreach (WorldTile tTile in tTiles)
		{
			if (tTile.data.tile_id <= pixels.Length - 1)
			{
				pixels[tTile.data.tile_id] = Color.clear;
			}
		}
		_tiles.Clear();
		createTextureNew();
	}
}
