using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
	public static MapGenTemplate template;

	private static int _width = 0;

	private static int _height = 0;

	private static WorldTile[,] _tilesMap;

	private static List<GeneratedRoom> _rooms = new List<GeneratedRoom>();

	private static bool reported_tryMakeBiomeSteps = false;

	private static MapGenValues gen_values => template.values;

	public static void clear()
	{
		_tilesMap = null;
		_rooms.Clear();
	}

	public static void prepare()
	{
		template = AssetManager.map_gen_templates.get(Config.current_map_template);
		_width = MapBox.width;
		_height = MapBox.height;
		_tilesMap = World.world.tiles_map;
		schedulePerlinNoiseMap();
		scheduleUpdateTileTypes();
		if (gen_values.forbidden_knowledge_start)
		{
			World.world.world_laws.enable("world_law_cursed_world");
			CursedSacrifice.loadAlreadyCursedState();
		}
		if (gen_values.remove_mountains)
		{
			SmoothLoader.add(delegate
			{
				removeMountains();
			}, "Normalize Ground");
		}
		if (template.special_anthill)
		{
			SmoothLoader.add(delegate
			{
				specialAnthill();
			}, "Anthill");
		}
		if (template.special_checkerboard)
		{
			SmoothLoader.add(delegate
			{
				specialCheckerBoard();
			}, "Checkerboard");
		}
		if (template.special_cubicles)
		{
			SmoothLoader.add(delegate
			{
				specialCubicles();
			}, "Cubicles");
		}
		scheduleRandomShapes(Randy.randomBool());
		if (template.perlin_replace.Count > 0)
		{
			foreach (PerlinReplaceContainer tOption in template.perlin_replace)
			{
				SmoothLoader.add(delegate
				{
					GeneratorTool.ApplyPerlinReplace(tOption);
				}, "Perlin Replace");
			}
		}
		SmoothLoader.add(delegate
		{
			World.world.map_chunk_manager.allDirty();
		}, "Map Chunk Manager (1/2)", pSkipFrame: false, 0.1f);
		SmoothLoader.add(delegate
		{
			World.world.map_chunk_manager.update(0f, pForce: true);
		}, "Map Chunk Manager (2/2)", pSkipFrame: false, 0.1f);
		if (gen_values.random_biomes)
		{
			SmoothLoader.add(delegate
			{
				generateBiomes();
			}, "Add Random Biome");
		}
		if (gen_values.add_mountain_edges)
		{
			SmoothLoader.add(delegate
			{
				addMountainEdges();
			}, "Add Mountain Edges");
		}
		if (template.freeze_mountains)
		{
			SmoothLoader.add(delegate
			{
				freezeMountainTops();
			}, "Freeze Mountain Tops");
		}
		if (gen_values.add_vegetation)
		{
			int vegTimes = 12;
			int vegSpread = World.world.tiles_list.Length / 20;
			for (int i = 0; i < vegTimes; i++)
			{
				SmoothLoader.add(delegate
				{
					AssetManager.tiles.setListTo(DepthGeneratorType.Gameplay);
					spawnVegetation(vegSpread);
				}, "Add Vegetation (" + (i + 1) + "/" + vegTimes + ")");
			}
		}
		if (gen_values.add_resources)
		{
			SmoothLoader.add(delegate
			{
				spawnResources();
			}, "Add Resources");
		}
	}

	private static void scheduleUpdateTileTypes()
	{
		int tileTypesSplit = 4;
		int tilesLoadedPerFrame = World.world.tiles_list.Length / tileTypesSplit;
		int loadedTiles = 0;
		for (int i = 0; i < tileTypesSplit; i++)
		{
			int tileAmount = Mathf.Min(World.world.tiles_list.Length - loadedTiles, tilesLoadedPerFrame);
			int startIndex = loadedTiles;
			loadedTiles += tileAmount;
			SmoothLoader.add(delegate
			{
				GeneratorTool.UpdateTileTypes(pGeneratorStage: true, startIndex, tileAmount);
			}, "Generate Tiles (" + loadedTiles + "/" + World.world.tiles_list.Length + ")", pSkipFrame: true);
		}
	}

	private static void scheduleRandomShapes(bool pSubstract)
	{
		if (gen_values.random_shapes_amount == 0)
		{
			return;
		}
		SmoothLoader.add(delegate
		{
			GeneratorTool.Init();
		}, "Perlin Random Shapes (Init)");
		for (int i = 0; i < gen_values.random_shapes_amount; i++)
		{
			SmoothLoader.add(delegate
			{
				GeneratorTool.ApplyRandomShape("height", 2f, 0.7f, pSubstract);
			}, "Perlin Random Shapes (" + (i + 1) + "/" + gen_values.random_shapes_amount + ")");
		}
	}

	private static void specialCubicles()
	{
		HashSet<MapChunk> chunks_used = new HashSet<MapChunk>();
		List<MapChunk> chunks_left = new List<MapChunk>();
		MapChunk[] chunks = World.world.map_chunk_manager.chunks;
		foreach (MapChunk tChunk in chunks)
		{
			chunks_left.Add(tChunk);
		}
		_rooms.Clear();
		while (chunks_left.Count > 0)
		{
			MapChunk tChunk2 = chunks_left.GetRandom();
			startCubicle(chunks_used, chunks_left, tChunk2);
		}
		createDoors();
	}

	private static void startCubicle(HashSet<MapChunk> pChunksUsed, List<MapChunk> pListLeft, MapChunk pStartRoomChunk)
	{
		MapChunk tNextChunk = pStartRoomChunk;
		List<MapChunk> tNewRoom = new List<MapChunk>();
		rememberChunk(tNextChunk, pChunksUsed, pListLeft, tNewRoom);
		int tMin = 2;
		int tMax = gen_values.cubicle_size + 2;
		int tWidth = Randy.randomInt(tMin, tMax);
		for (int i = 0; i < tWidth; i++)
		{
			if (tNextChunk == null)
			{
				break;
			}
			tNextChunk = tNextChunk.chunk_right;
			if (tNextChunk != null && !pChunksUsed.Contains(tNextChunk))
			{
				rememberChunk(tNextChunk, pChunksUsed, pListLeft, tNewRoom);
			}
		}
		tNextChunk = pStartRoomChunk;
		tWidth = Randy.randomInt(tMin, tMax);
		for (int j = 0; j < tWidth; j++)
		{
			if (tNextChunk == null)
			{
				break;
			}
			tNextChunk = tNextChunk.chunk_left;
			if (tNextChunk != null && !pChunksUsed.Contains(tNextChunk))
			{
				rememberChunk(tNextChunk, pChunksUsed, pListLeft, tNewRoom);
			}
		}
		List<MapChunk> tInitialLine = new List<MapChunk>();
		tInitialLine.AddRange(tNewRoom);
		List<MapChunk> tPrevLine = new List<MapChunk>();
		tPrevLine.AddRange(tInitialLine);
		int tHeight = Randy.randomInt(tMin, tMax);
		for (int k = 0; k < tHeight; k++)
		{
			List<MapChunk> tNewLine = new List<MapChunk>();
			bool tCanAddBottom = true;
			foreach (MapChunk tChunk in tPrevLine)
			{
				if (tChunk.chunk_down == null)
				{
					tCanAddBottom = false;
					k = tHeight;
					break;
				}
				if (pChunksUsed.Contains(tChunk.chunk_down))
				{
					tCanAddBottom = false;
					k = tHeight;
					break;
				}
				tNewLine.Add(tChunk);
			}
			if (!tCanAddBottom)
			{
				continue;
			}
			tPrevLine.Clear();
			foreach (MapChunk tChunk2 in tNewLine)
			{
				rememberChunk(tChunk2.chunk_down, pChunksUsed, pListLeft, tNewRoom);
				tPrevLine.Add(tChunk2.chunk_down);
			}
		}
		tPrevLine.Clear();
		tPrevLine.AddRange(tInitialLine);
		tHeight = Randy.randomInt(tMin, tMax);
		for (int l = 0; l < tHeight; l++)
		{
			List<MapChunk> tNewLine2 = new List<MapChunk>();
			bool tCanAddBottom2 = true;
			foreach (MapChunk tChunk3 in tPrevLine)
			{
				if (tChunk3.chunk_up == null)
				{
					tCanAddBottom2 = false;
					l = tHeight;
					break;
				}
				if (pChunksUsed.Contains(tChunk3.chunk_up))
				{
					tCanAddBottom2 = false;
					l = tHeight;
					break;
				}
				tNewLine2.Add(tChunk3);
			}
			if (!tCanAddBottom2)
			{
				continue;
			}
			tPrevLine.Clear();
			foreach (MapChunk tChunk4 in tNewLine2)
			{
				rememberChunk(tChunk4.chunk_up, pChunksUsed, pListLeft, tNewRoom);
				tPrevLine.Add(tChunk4.chunk_up);
			}
		}
		finishRoom(tNewRoom);
	}

	private static void rememberChunk(MapChunk pChunk, HashSet<MapChunk> pChunksUsed, List<MapChunk> pListLeft, List<MapChunk> pNewRoom)
	{
		pChunksUsed.Add(pChunk);
		pListLeft.Remove(pChunk);
		pNewRoom.Add(pChunk);
	}

	private static void finishRoom(List<MapChunk> pChunks)
	{
		BiomeAsset tBiome = BiomeLibrary.pool_biomes.GetRandom();
		TileType tType = TileLibrary.soil_high;
		if (Randy.randomBool())
		{
			tType = TileLibrary.soil_low;
		}
		WorldTile t_u_l = World.world.GetTileSimple(0, 0);
		WorldTile t_u_r = World.world.GetTileSimple(MapBox.width - 1, 0);
		WorldTile t_d_l = World.world.GetTileSimple(0, MapBox.height - 1);
		WorldTile t_d_r = World.world.GetTileSimple(MapBox.width - 1, MapBox.height - 1);
		WorldTile tRoom_u_l = null;
		WorldTile tRoom_u_r = null;
		WorldTile tRoom_d_l = null;
		WorldTile tRoom_d_r = null;
		float best_dist_u_l = 0f;
		float best_dist_u_r = 0f;
		float best_dist_d_l = 0f;
		float best_dist_d_r = 0f;
		for (int i = 0; i < pChunks.Count; i++)
		{
			WorldTile[] tTiles = pChunks[i].tiles;
			int tCount = tTiles.Length;
			for (int j = 0; j < tCount; j++)
			{
				WorldTile tTile = tTiles[j];
				MapAction.terraformTile(tTile, tType, null);
				if (gen_values.random_biomes)
				{
					DropsLibrary.useSeedOn(tTile, tBiome.getTileLow(), tBiome.getTileHigh());
				}
				float dist_u_l = Toolbox.DistTile(t_u_l, tTile);
				float dist_u_r = Toolbox.DistTile(t_u_r, tTile);
				float dist_d_l = Toolbox.DistTile(t_d_l, tTile);
				float dist_d_r = Toolbox.DistTile(t_d_r, tTile);
				if (tRoom_u_l == null || dist_u_l < best_dist_u_l)
				{
					tRoom_u_l = tTile;
					best_dist_u_l = dist_u_l;
				}
				if (tRoom_u_r == null || dist_u_r < best_dist_u_r)
				{
					tRoom_u_r = tTile;
					best_dist_u_r = dist_u_r;
				}
				if (tRoom_d_l == null || dist_d_l < best_dist_d_l)
				{
					tRoom_d_l = tTile;
					best_dist_d_l = dist_d_l;
				}
				if (tRoom_d_r == null || dist_d_r < best_dist_d_r)
				{
					tRoom_d_r = tTile;
					best_dist_d_r = dist_d_r;
				}
			}
		}
		GeneratedRoom tRoomObject = new GeneratedRoom();
		tRoomObject.id_debug = _rooms.Count;
		_rooms.Add(tRoomObject);
		tRoomObject.edges_up = fillTiles(tRoom_u_l, tRoom_u_r, TileLibrary.mountains);
		tRoomObject.edges_down = fillTiles(tRoom_d_l, tRoom_d_r, TileLibrary.mountains);
		tRoomObject.edges_left = fillTiles(tRoom_u_l, tRoom_d_l, TileLibrary.mountains);
		tRoomObject.edges_right = fillTiles(tRoom_d_r, tRoom_u_r, TileLibrary.mountains);
	}

	private static void createDoors()
	{
		foreach (GeneratedRoom room in _rooms)
		{
			makeDoor(room.edges_down);
			makeDoor(room.edges_left);
			makeDoor(room.edges_right);
			makeDoor(room.edges_up);
		}
	}

	private static void makeDoor(List<WorldTile> pTiles)
	{
		foreach (WorldTile pTile in pTiles)
		{
			if (pTile.main_type != TileLibrary.mountains)
			{
				return;
			}
		}
		WorldTile tDoorTile;
		if (pTiles.Count > 3)
		{
			int tRandomIndex = Randy.randomInt(3, pTiles.Count - 3);
			tDoorTile = pTiles[tRandomIndex];
		}
		else
		{
			int tIndex = pTiles.Count / 2;
			tDoorTile = pTiles[tIndex];
		}
		MapAction.terraformTile(tDoorTile, TileLibrary.hills, null);
		WorldTile[] neighboursAll = tDoorTile.neighboursAll;
		foreach (WorldTile tNTile in neighboursAll)
		{
			if (tNTile.main_type == TileLibrary.mountains)
			{
				MapAction.terraformTile(tNTile, TileLibrary.hills, null);
			}
		}
	}

	private static void specialCheckerBoard()
	{
		BiomeAsset tBiome1 = BiomeLibrary.pool_biomes.GetRandom();
		BiomeAsset tBiome2 = BiomeLibrary.pool_biomes.GetRandom();
		MapChunk[] tChunks = World.world.map_chunk_manager.chunks;
		foreach (MapChunk tChunk in tChunks)
		{
			WorldTile[] tTiles = tChunk.tiles;
			if ((tChunk.x + tChunk.y) % 2 == 0)
			{
				int tCount = tTiles.Length;
				for (int j = 0; j < tCount; j++)
				{
					WorldTile tTile = tTiles[j];
					MapAction.terraformTile(tTile, TileLibrary.soil_high, null);
					if (gen_values.random_biomes)
					{
						DropsLibrary.useSeedOn(tTile, tBiome1.getTileLow(), tBiome1.getTileHigh());
					}
				}
				continue;
			}
			int tCount2 = tTiles.Length;
			for (int k = 0; k < tCount2; k++)
			{
				WorldTile tTile2 = tTiles[k];
				MapAction.terraformTile(tTile2, TileLibrary.soil_low, null);
				if (gen_values.random_biomes)
				{
					DropsLibrary.useSeedOn(tTile2, tBiome2.getTileLow(), tBiome2.getTileHigh());
				}
			}
		}
	}

	private static void specialAnthill()
	{
		WorldTile[] tiles_list = World.world.tiles_list;
		for (int i = 0; i < tiles_list.Length; i++)
		{
			MapAction.terraformTile(tiles_list[i], TileLibrary.mountains, null);
		}
		List<TileZone> list = new List<TileZone>();
		List<WorldTile> tTunnels = new List<WorldTile>();
		ZoneCalculator tZones = World.world.zone_calculator;
		int tOffset_x = tZones.zones_total_x / 10 + 1;
		int tOffset_y = tZones.zones_total_y / 10 + 1;
		TileZone t_U_L = tZones.map[tOffset_x, tOffset_y];
		TileZone t_U_R = tZones.map[tZones.zones_total_x - tOffset_x, tOffset_y];
		TileZone t_D_R = tZones.map[tZones.zones_total_x - tOffset_x, tZones.zones_total_y - tOffset_y];
		TileZone t_D_L = tZones.map[tOffset_x, tZones.zones_total_y - tOffset_y];
		makeJailRoom(list, tZones.map[tZones.zones_total_x / 2, tZones.zones_total_y / 2]);
		makeJailRoom(list, tZones.map[tZones.zones_total_x / 2, tZones.zones_total_y / 2]);
		makeJailRoom(list, t_U_L);
		makeJailRoom(list, t_U_L);
		makeJailRoom(list, t_U_R);
		makeJailRoom(list, t_U_R);
		makeJailRoom(list, t_D_R);
		makeJailRoom(list, t_D_R);
		makeJailRoom(list, t_D_L);
		makeJailRoom(list, t_D_L);
		makeWay(t_U_L, t_U_R, tTunnels);
		makeWay(t_D_L, t_D_R, tTunnels);
		makeWay(t_U_L, t_D_L, tTunnels);
		makeWay(t_D_R, t_U_R, tTunnels);
		makeWay(t_D_R, t_U_L, tTunnels);
		foreach (TileZone item in list)
		{
			carveZone(item);
		}
		foreach (WorldTile item2 in tTunnels)
		{
			carveTunnel(item2);
		}
	}

	private static List<WorldTile> fillTiles(WorldTile pTile1, WorldTile pTile2, TileType pType)
	{
		List<WorldTile> list = PathfinderTools.raycast(pTile1, pTile2, 1f);
		List<WorldTile> tNewList = new List<WorldTile>(list);
		list.Clear();
		foreach (WorldTile item in tNewList)
		{
			MapAction.terraformTile(item, pType, null);
		}
		return tNewList;
	}

	private static void makeWay(TileZone tZone1, TileZone tZone2, List<WorldTile> pTunnels)
	{
		List<WorldTile> tRaycastResult = PathfinderTools.raycast(tZone1.centerTile, tZone2.centerTile);
		foreach (WorldTile item in tRaycastResult)
		{
			WorldTile[] neighboursAll = item.neighboursAll;
			foreach (WorldTile tNeighbour in neighboursAll)
			{
				MapAction.terraformTile(tNeighbour, TileLibrary.soil_high, null);
				pTunnels.Add(tNeighbour);
			}
		}
		tRaycastResult.Clear();
	}

	private static void carveTunnel(WorldTile pTile)
	{
		for (int i = 0; i < 10; i++)
		{
			WorldTile tTile = pTile.neighbours.GetRandom();
			for (int tN = 10; tN > 0; tN--)
			{
				WorldTile tNeighbour = tTile.neighbours.GetRandom();
				if (tNeighbour.Type.rocks)
				{
					MapAction.terraformTile(tTile, TileLibrary.soil_high, null);
					tTile = tNeighbour;
				}
			}
		}
	}

	private static void carveZone(TileZone pZone)
	{
		for (int i = 0; i < 20; i++)
		{
			WorldTile tTile = pZone.tiles.GetRandom();
			for (int tN = 15; tN > 0; tN--)
			{
				WorldTile tNeighbour = tTile.neighbours.GetRandom();
				if (tNeighbour.Type.rocks)
				{
					MapAction.terraformTile(tTile, TileLibrary.soil_high, null);
					tTile = tNeighbour;
				}
			}
		}
	}

	private static void makeJailRoom(List<TileZone> pZones, TileZone pStartZone)
	{
		int tRoomSize = World.world.zone_calculator.zones.Count / 10;
		TileZone tZone = pStartZone;
		if (tZone.world_edge)
		{
			return;
		}
		for (int i = 0; i < tRoomSize; i++)
		{
			if (tZone.world_edge)
			{
				tZone = tZone.neighbours.GetRandom();
				continue;
			}
			WorldTile[] tTiles = tZone.tiles;
			int tCount = tTiles.Length;
			for (int j = 0; j < tCount; j++)
			{
				MapAction.terraformTile(tTiles[j], TileLibrary.soil_high, null);
			}
			pZones.Add(tZone);
			tZone = tZone.neighbours.GetRandom();
		}
	}

	private static void schedulePerlinNoiseMap()
	{
		scheduleRandomShapes(pSubstract: true);
		if (gen_values.main_perlin_noise_stage && gen_values.perlin_scale_stage_1 > 0)
		{
			SmoothLoader.add(delegate
			{
				int num = Randy.randomInt(0, 1000000);
				int num2 = Randy.randomInt(0, 1000000);
				GeneratorTool.ApplyPerlinNoise(_tilesMap, _width, _height, num, num2, 1f, 1f * (float)gen_values.perlin_scale_stage_1);
			}, "Perlin Noise", pSkipFrame: true);
		}
		if (template.force_height_to > 0)
		{
			SmoothLoader.add(delegate
			{
				forceHeight();
			}, "Add Height");
		}
		if (gen_values.add_center_gradient_land)
		{
			SmoothLoader.add(delegate
			{
				addCenterGradient();
			}, "Center Gradient");
		}
		if (gen_values.center_gradient_mountains)
		{
			SmoothLoader.add(delegate
			{
				addCenterMountains();
			}, "Center Mountains");
		}
		if (gen_values.add_center_lake)
		{
			SmoothLoader.add(delegate
			{
				addCenterLake();
			}, "Center Lake");
		}
		if (gen_values.perlin_noise_stage_2 && gen_values.perlin_scale_stage_2 > 0)
		{
			SmoothLoader.add(delegate
			{
				float num = gen_values.perlin_scale_stage_2;
				int num2 = Randy.randomInt(0, 1000000);
				int num3 = Randy.randomInt(0, 1000000);
				GeneratorTool.ApplyPerlinNoise(_tilesMap, _width, _height, num2, num3, 0.2f, 4f * num, pSubtract: true);
			}, "Perlin Noise (1)");
		}
		if (gen_values.perlin_noise_stage_3 && gen_values.perlin_scale_stage_3 > 0)
		{
			SmoothLoader.add(delegate
			{
				float num = gen_values.perlin_scale_stage_3;
				int num2 = Randy.randomInt(0, 1000000);
				int num3 = Randy.randomInt(0, 1000000);
				GeneratorTool.ApplyPerlinNoise(_tilesMap, _width, _height, num2, num3, 0.1f, num * 10f, pSubtract: true);
			}, "Perlin Noise (2)");
		}
		if (gen_values.low_ground)
		{
			SmoothLoader.add(delegate
			{
				lowGround();
			}, "Lower Ground");
		}
		if (gen_values.high_ground)
		{
			SmoothLoader.add(delegate
			{
				highGround();
			}, "High Ground");
		}
		scheduleRandomShapes(pSubstract: true);
		if (gen_values.ring_effect)
		{
			SmoothLoader.add(delegate
			{
				GeneratorTool.ApplyRingEffect();
			}, "Perlin Ring", pSkipFrame: true);
		}
		if (gen_values.gradient_round_edges)
		{
			SmoothLoader.add(delegate
			{
				MapEdges.AddEdgeGradientCircle(World.world.tiles_map, "height");
			}, "Gradient Circle Edges", pSkipFrame: true);
		}
		if (gen_values.square_edges)
		{
			SmoothLoader.add(delegate
			{
				MapEdges.AddEdgeSquare(World.world.tiles_map, "height");
			}, "Gradient Circle Edges", pSkipFrame: true);
		}
	}

	private static void forceHeight()
	{
		WorldTile[] tiles_list = World.world.tiles_list;
		for (int i = 0; i < tiles_list.Length; i++)
		{
			tiles_list[i].Height = template.force_height_to;
		}
	}

	private static void removeMountains()
	{
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.Type.rocks)
			{
				MapAction.decreaseTile(tTile, pDamage: false);
			}
			if (tTile.Type.rocks)
			{
				MapAction.decreaseTile(tTile, pDamage: false);
			}
		}
	}

	private static void lowGround()
	{
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.Height > 150)
			{
				tTile.Height -= 50;
			}
			if (tTile.Height > 130)
			{
				tTile.Height -= 20;
			}
		}
	}

	private static void highGround()
	{
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			if (tTile.Height < 20)
			{
				tTile.Height += 80;
			}
			else if (tTile.Height < 120)
			{
				tTile.Height += 40;
			}
		}
	}

	private static void addCenterGradient()
	{
		WorldTile tCenter = World.world.tiles_map[MapBox.width / 2, MapBox.height / 2];
		float tMaxMod = 0.9f;
		float tGradientMod = 0.6f;
		float tMaxCenter = (float)(MapBox.width / 2) * tMaxMod;
		float tGradient = (float)(MapBox.width / 2) * tGradientMod;
		float tDiff = tMaxCenter - tGradient;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			float tDist = Toolbox.DistTile(tTile, tCenter);
			if (!(tDist > tMaxCenter))
			{
				float tMod = (tMaxCenter - tDist) / tDiff;
				int tNewHeight = (int)(45f * tMod);
				tTile.Height += tNewHeight;
			}
		}
	}

	private static void addCenterLake()
	{
		WorldTile tCenter = World.world.tiles_map[MapBox.width / 2, MapBox.height / 2];
		float tMaxMod = 0.6f;
		float tGradientMod = 0.2f;
		float tMaxCenter = (float)(MapBox.width / 2) * tMaxMod;
		float tGradient = (float)(MapBox.width / 2) * tGradientMod;
		float tDiff = tMaxCenter - tGradient;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile tTile in tiles_list)
		{
			float tDist = Toolbox.DistTile(tTile, tCenter);
			if (!(tDist > tMaxCenter))
			{
				float tVal = tMaxCenter - tDist;
				float tMod = 1f - tVal / tDiff;
				int tNewHeight = (int)((float)tTile.Height * tMod);
				tTile.Height = tNewHeight;
			}
		}
	}

	private static void addCenterMountains()
	{
		WorldTile tCenter = World.world.tiles_map[MapBox.width / 2, MapBox.height / 2];
		float tMaxMod = 0.3f;
		float tGradientMod = 0f;
		float tMaxCenter = (float)(MapBox.width / 2) * tMaxMod;
		float tGradient = (float)(MapBox.width / 2) * tGradientMod;
		float tDiff = tMaxCenter - tGradient;
		WorldTile[] tiles_list = World.world.tiles_list;
		foreach (WorldTile obj in tiles_list)
		{
			float tDist = Toolbox.DistTile(obj, tCenter);
			float tMod = (tMaxCenter - tDist) / tDiff;
			int tNewHeight = (int)(75f * tMod);
			obj.Height -= tNewHeight;
		}
	}

	private static void generateBiomes()
	{
		HashSetWorldTile tTilesSoilHashset = new HashSetWorldTile();
		for (int i = 0; i < World.world.tiles_list.Length; i++)
		{
			WorldTile tTile = World.world.tiles_list[i];
			if (tTile.Type.soil)
			{
				tTilesSoilHashset.Add(tTile);
			}
		}
		using ListPool<WorldTile> tTempSoilTiles = new ListPool<WorldTile>(tTilesSoilHashset.Count);
		bool tRecreateList = true;
		while (tTilesSoilHashset.Count > 0)
		{
			if (tRecreateList)
			{
				tRecreateList = false;
				recreateSoilList(tTilesSoilHashset, tTempSoilTiles);
			}
			WorldTile tStartTile = tTempSoilTiles.Last();
			BiomeAsset tBiome = BiomeLibrary.pool_biomes.GetRandom();
			int tMaxSteps = tryMakeBiomeSteps(tStartTile, tBiome);
			if (tMaxSteps == 0)
			{
				tTempSoilTiles.Pop();
				continue;
			}
			tryMakeBiome(tStartTile, tTilesSoilHashset, tMaxSteps, tBiome);
			tRecreateList = true;
		}
	}

	private static void recreateSoilList(HashSetWorldTile pHashSet, ListPool<WorldTile> pTempSoilTiles)
	{
		pTempSoilTiles.Clear();
		pTempSoilTiles.AddRange(pHashSet);
		pTempSoilTiles.Shuffle();
	}

	private static int tryMakeBiomeSteps(WorldTile pStartTile, BiomeAsset pBiome)
	{
		if (!reported_tryMakeBiomeSteps)
		{
			if (pBiome == null)
			{
				Debug.Log("pBiome is null");
				reported_tryMakeBiomeSteps = true;
			}
			if (pStartTile == null)
			{
				Debug.Log("pStartTile is null");
				reported_tryMakeBiomeSteps = true;
			}
			if (pStartTile.region == null)
			{
				Debug.Log("pStartTile.region is null");
				reported_tryMakeBiomeSteps = true;
			}
			if (pStartTile.region.island == null)
			{
				Debug.Log("pStartTile.region.island is null");
				reported_tryMakeBiomeSteps = true;
			}
		}
		int tIslandTileCount = pStartTile.region.island.getTileCount();
		int tMaxSteps = ((tIslandTileCount < 400) ? tIslandTileCount : ((tIslandTileCount >= 600) ? (tIslandTileCount / 3) : (tIslandTileCount / 2)));
		if (tMaxSteps > pBiome.generator_max_size && pBiome.generator_max_size != 0)
		{
			tMaxSteps = pBiome.generator_max_size;
		}
		return tMaxSteps;
	}

	private static void tryMakeBiome(WorldTile pStartTile, HashSetWorldTile pSoilTiles, int pMaxSteps, BiomeAsset pBiome)
	{
		int tCurrentSteps = 0;
		using ListPool<WorldTile> tWave = new ListPool<WorldTile>(pMaxSteps);
		HashSetWorldTile tCheckedTiles = new HashSetWorldTile();
		tWave.Add(pStartTile);
		tCheckedTiles.Add(pStartTile);
		while (tWave.Count > 0 && tCurrentSteps < pMaxSteps)
		{
			tWave.ShuffleLast();
			WorldTile tNewTile = tWave.Pop();
			if (tNewTile.isTileRank(TileRank.Low))
			{
				tNewTile.setTopTileType(pBiome.getTileLow());
			}
			else
			{
				tNewTile.setTopTileType(pBiome.getTileHigh());
			}
			pSoilTiles.Remove(tNewTile);
			tCurrentSteps++;
			for (int i = 0; i < tNewTile.neighboursAll.Length; i++)
			{
				WorldTile tTile = tNewTile.neighboursAll[i];
				if (!tCheckedTiles.Contains(tTile) && tTile.Type.soil)
				{
					tWave.Add(tTile);
					tCheckedTiles.Add(tTile);
				}
			}
		}
		if (tCurrentSteps <= 10)
		{
			removeSmallBiomePatches(tCheckedTiles, pStartTile);
		}
	}

	private static void removeSmallBiomePatches(HashSetWorldTile pTiles, WorldTile pStartTile)
	{
		WorldTile tCopyFrom = null;
		WorldTile[] neighboursAll = pStartTile.neighboursAll;
		foreach (WorldTile tTile in neighboursAll)
		{
			if (tTile.Type.is_biome && !tTile.Type.biome_asset.special_biome && tTile.Type.biome_asset != pStartTile.Type.biome_asset)
			{
				tCopyFrom = tTile;
				break;
			}
		}
		if (tCopyFrom == null)
		{
			return;
		}
		BiomeAsset tBiome = tCopyFrom.top_type.biome_asset;
		foreach (WorldTile tCheckedTile in pTiles)
		{
			TopTileType tBiomeTileType = tBiome.getTile(tCheckedTile);
			tCheckedTile.setTopTileType(tBiomeTileType);
		}
	}

	private static void addMountainEdges()
	{
		int tOffset_x = 0;
		int tOffset_y = 0;
		WorldTile tileSimple = World.world.GetTileSimple(tOffset_x, tOffset_y);
		WorldTile t_U_R = World.world.GetTileSimple(MapBox.width - tOffset_x - 1, tOffset_y);
		WorldTile t_D_R = World.world.GetTileSimple(MapBox.width - tOffset_x - 1, MapBox.height - tOffset_y - 1);
		WorldTile t_D_L = World.world.GetTileSimple(tOffset_x, MapBox.height - tOffset_y - 1);
		fillTiles(tileSimple, t_U_R, TileLibrary.mountains);
		fillTiles(t_D_L, t_D_R, TileLibrary.mountains);
		fillTiles(tileSimple, t_D_L, TileLibrary.mountains);
		fillTiles(t_D_R, t_U_R, TileLibrary.mountains);
	}

	private static void freezeMountainTops()
	{
		for (int i = 0; i < World.world.tiles_list.Length; i++)
		{
			WorldTile tTile = World.world.tiles_list[i];
			if (tTile.Type.IsType("mountains") && tTile.Height > 220)
			{
				tTile.freeze();
			}
		}
	}

	private static void spawnVegetation(int pAmount)
	{
		for (int i = 0; i < pAmount; i++)
		{
			WorldTile tObjTile = World.world.tiles_list.GetRandom();
			if (!tObjTile.Type.ground || tObjTile.zone.countBuildingsType(BuildingList.Trees) >= 3)
			{
				continue;
			}
			BiomeAsset tBiomeAsset = tObjTile.Type.biome_asset;
			if (tBiomeAsset != null && tBiomeAsset.grow_vegetation_auto)
			{
				switch (Randy.randomInt(0, 3))
				{
				case 0:
					BuildingActions.tryGrowVegetationRandom(tObjTile, VegetationType.Plants, pOnStart: true);
					break;
				case 1:
					BuildingActions.tryGrowVegetationRandom(tObjTile, VegetationType.Trees, pOnStart: true);
					break;
				case 2:
					BuildingActions.tryGrowVegetationRandom(tObjTile, VegetationType.Bushes, pOnStart: true);
					break;
				}
			}
		}
	}

	private static void spawnResources()
	{
		BuildingActions.spawnResource(World.world.tiles_list.Length / 1000 / 2 / 2, "fruit_bush", pRandomSize: false);
	}
}
