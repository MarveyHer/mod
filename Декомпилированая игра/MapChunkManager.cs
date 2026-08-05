using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class MapChunkManager
{
	private readonly Color _color_1_gray = Color.gray;

	private readonly Color _color_2_white = Color.white;

	private MapChunk[,] _map;

	public MapChunk[] chunks = new MapChunk[0];

	private readonly List<MapChunk> _dirty_chunks_regions = new List<MapChunk>();

	private readonly List<MapChunk> _dirty_chunks_links = new List<MapChunk>();

	private int m_dirtyChunks;

	private static int m_dirtyCorners;

	private static int m_newRegions;

	public static int m_newLinks;

	internal static int m_newIslands;

	internal static int m_dirtyIslands;

	private float _timer = 0.4f;

	private int _get_amount_x;

	private int _amount_y;

	private readonly List<MapChunk> _last_dirty_chunks = new List<MapChunk>();

	private readonly List<MapChunk> _last_dirty_links = new List<MapChunk>();

	private readonly HashSet<MapRegion> _region_set = new HashSet<MapRegion>();

	private readonly List<TileIsland> _temp_dirty_neighbours_islands = new List<TileIsland>();

	public int amount_x => _get_amount_x;

	private bool _is_parallel_enabled => DebugConfig.isOn(DebugOption.ParallelChunks);

	private bool _batches_enabled => DebugConfig.isOn(DebugOption.ChunkBatches);

	public void checkDiagnosticRegions()
	{
		diagnosticRegions();
	}

	public void update(float pElapsed, bool pForce = false)
	{
		if (_timer > 0f && !pForce)
		{
			_timer -= pElapsed;
		}
		else
		{
			updateDirty();
		}
	}

	private void diagnosticRegions()
	{
		HashSet<MapRegion> tRegionMain = new HashSet<MapRegion>();
		HashSet<MapRegion> tRegionNeighbour = new HashSet<MapRegion>();
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			foreach (MapRegion tReg in tChunks[i].regions)
			{
				if (tReg.tiles.Count == 0 || tReg.chunk == null)
				{
					tRegionMain.Add(tReg);
				}
				foreach (MapRegion tRegNeighbour in tReg.neighbours)
				{
					if (tRegNeighbour.tiles.Count == 0 || tRegNeighbour.chunk == null)
					{
						tRegionNeighbour.Add(tReg);
					}
				}
			}
		}
		if (tRegionMain.Count > 0 || tRegionNeighbour.Count > 0)
		{
			Debug.LogError("Something is wrong with regions");
			Debug.LogError("tRegionMain: " + tRegionMain.Count);
			Debug.LogError("tRegionNeighbour: " + tRegionNeighbour.Count);
		}
	}

	public void prepare()
	{
		int tMod = 4;
		_get_amount_x = Config.ZONE_AMOUNT_X * tMod;
		_amount_y = Config.ZONE_AMOUNT_Y * tMod;
		_map = new MapChunk[_get_amount_x, _amount_y];
		int tLen = _get_amount_x * _amount_y;
		if (tLen != chunks.Length)
		{
			chunks = new MapChunk[tLen];
		}
		else
		{
			Array.Clear(chunks, 0, chunks.Length);
		}
		int tId = 0;
		int iZone = 0;
		for (int yy = 0; yy < _amount_y; yy++)
		{
			for (int xx = 0; xx < _get_amount_x; xx++)
			{
				MapChunk tChunk = new MapChunk();
				tChunk.id = tId++;
				tChunk.x = xx;
				tChunk.y = yy;
				_map[xx, yy] = tChunk;
				if ((xx + yy) % 2 == 0)
				{
					tChunk.color = _color_1_gray;
				}
				else
				{
					tChunk.color = _color_2_white;
				}
				chunks[iZone] = tChunk;
				fill(tChunk);
				iZone++;
			}
		}
		fillAndLinkTileZones();
		generateNeighbours();
		generateEdgeConnections();
	}

	private void generateEdgeConnections()
	{
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			tChunks[i].generateEdgeConnections();
		}
	}

	private void fillAndLinkTileZones()
	{
		for (int i = 0; i < World.world.zone_calculator.zones.Count; i++)
		{
			TileZone tZone = World.world.zone_calculator.zones[i];
			tZone.chunk.zones.Add(tZone);
		}
	}

	private void fill(MapChunk pChunk)
	{
		int tChunkActual = 16;
		int tStartX = pChunk.x * tChunkActual;
		int tStartY = pChunk.y * tChunkActual;
		for (int xx = 0; xx < tChunkActual; xx++)
		{
			for (int yy = 0; yy < tChunkActual; yy++)
			{
				WorldTile tTile = World.world.GetTileSimple(xx + tStartX, yy + tStartY);
				tTile.chunk = pChunk;
				pChunk.addTile(tTile, xx, yy);
			}
		}
		pChunk.world_center_x = tStartX + 8;
		pChunk.world_center_y = tStartY + 8;
		for (int i = 0; i < 16; i++)
		{
			WorldTile tTile = World.world.GetTileSimple(i + tStartX, tStartY);
			pChunk.bounds_down.Add(tTile);
		}
		for (int j = 0; j < 16; j++)
		{
			WorldTile tTile = World.world.GetTileSimple(tStartX, tStartY + j);
			pChunk.bounds_left.Add(tTile);
		}
		for (int k = 0; k < 16; k++)
		{
			WorldTile tTile = World.world.GetTileSimple(tStartX + 16 - 1, tStartY + k);
			pChunk.bounds_right.Add(tTile);
		}
		for (int l = 0; l < 16; l++)
		{
			WorldTile tTile = World.world.GetTileSimple(l + tStartX, tStartY + 16 - 1);
			pChunk.bounds_up.Add(tTile);
		}
		pChunk.edge_up_left = pChunk.bounds_up[0];
		pChunk.edge_up_right = pChunk.bounds_up[15];
		pChunk.edge_down_left = pChunk.bounds_down[0];
		pChunk.edge_down_right = pChunk.bounds_down[15];
		pChunk.combineEdges();
	}

	private void generateNeighbours()
	{
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		using ListPool<MapChunk> tNeighbours = new ListPool<MapChunk>(4);
		using ListPool<MapChunk> tNeighboursAll = new ListPool<MapChunk>(8);
		for (int i = 0; i < tLen; i++)
		{
			MapChunk tObj = tChunks[i];
			MapChunk tNeighbour = get(tObj.x - 1, tObj.y);
			tObj.addNeighbour(tNeighbour, TileDirection.Left, tNeighbours, tNeighboursAll);
			tNeighbour = get(tObj.x + 1, tObj.y);
			tObj.addNeighbour(tNeighbour, TileDirection.Right, tNeighbours, tNeighboursAll);
			tNeighbour = get(tObj.x, tObj.y - 1);
			tObj.addNeighbour(tNeighbour, TileDirection.Down, tNeighbours, tNeighboursAll);
			tNeighbour = get(tObj.x, tObj.y + 1);
			tObj.addNeighbour(tNeighbour, TileDirection.Up, tNeighbours, tNeighboursAll);
			tNeighbour = get(tObj.x - 1, tObj.y - 1);
			tObj.addNeighbour(tNeighbour, TileDirection.Null, tNeighbours, tNeighboursAll, pDiagonal: true);
			tNeighbour = get(tObj.x - 1, tObj.y + 1);
			tObj.addNeighbour(tNeighbour, TileDirection.Null, tNeighbours, tNeighboursAll, pDiagonal: true);
			tNeighbour = get(tObj.x + 1, tObj.y - 1);
			tObj.addNeighbour(tNeighbour, TileDirection.Null, tNeighbours, tNeighboursAll, pDiagonal: true);
			tNeighbour = get(tObj.x + 1, tObj.y + 1);
			tObj.addNeighbour(tNeighbour, TileDirection.Null, tNeighbours, tNeighboursAll, pDiagonal: true);
			tObj.neighbours = tNeighbours.ToArray();
			tObj.neighbours_all = tNeighboursAll.ToArray();
			tNeighbours.Clear();
			tNeighboursAll.Clear();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MapChunk get(int pX, int pY)
	{
		if (pX < 0 || pX >= _get_amount_x || pY < 0 || pY >= _amount_y)
		{
			return null;
		}
		return _map[pX, pY];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MapChunk get(ref Vector2Int pPos)
	{
		if (pPos.x < 0 || pPos.x >= _get_amount_x || pPos.y < 0 || pPos.y >= _amount_y)
		{
			return null;
		}
		return _map[pPos.x, pPos.y];
	}

	public MapChunk getByID(int pID)
	{
		return chunks[pID];
	}

	public void clearAll()
	{
		World.world.islands_calculator.clear();
		_dirty_chunks_links.Clear();
		_dirty_chunks_regions.Clear();
		int tLen = chunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			chunks[i].clearRegions();
		}
	}

	public void clean()
	{
		clearAll();
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			tChunks[i].Dispose();
		}
		Array.Clear(chunks, 0, chunks.Length);
	}

	public void setAllLinksDirty()
	{
		MapChunk[] array = chunks;
		foreach (MapChunk tChunk in array)
		{
			setDirty(tChunk, pRegions: false);
		}
	}

	public void setDirty(MapChunk pChunk, bool pRegions = true, bool pLinks = true)
	{
		if (pRegions && !pChunk.dirty_regions)
		{
			pChunk.dirty_regions = true;
			_dirty_chunks_regions.Add(pChunk);
		}
		if (pLinks && !pChunk.dirty_links)
		{
			pChunk.dirty_links = true;
			_dirty_chunks_links.Add(pChunk);
		}
	}

	public void allDirty()
	{
		_dirty_chunks_links.Clear();
		_dirty_chunks_regions.Clear();
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			MapChunk obj = tChunks[i];
			obj.dirty_links = true;
			obj.dirty_regions = true;
		}
		_dirty_chunks_links.AddRange(chunks);
		_dirty_chunks_regions.AddRange(chunks);
	}

	private bool isAllChunksDirty()
	{
		return chunks.Length == _dirty_chunks_regions.Count;
	}

	private void updateDirty()
	{
		if (!DebugConfig.isOn(DebugOption.SystemUpdateDirtyChunks) || (!isAllChunksDirty() && World.world.isActionHappening()) || (_dirty_chunks_links.Count == 0 && _dirty_chunks_regions.Count == 0))
		{
			return;
		}
		Bench.bench("chunks", "chunks_total");
		_timer = 0.4f;
		m_dirtyChunks = _dirty_chunks_regions.Count;
		m_newRegions = 0;
		m_newLinks = 0;
		m_newIslands = 0;
		m_dirtyIslands = 0;
		m_dirtyCorners = 0;
		MapRegion.created_time_last = Time.time;
		World.world.islands_calculator.prepareCalc();
		Bench.bench("clear_regions", "chunks");
		calc_clearRegions();
		Bench.benchEnd("clear_regions", "chunks", pSaveCounter: true, _dirty_chunks_links.Count);
		Bench.bench("clear_dirty_islands", "chunks");
		World.world.islands_calculator.clearDirty();
		Bench.benchEnd("clear_dirty_islands", "chunks", pSaveCounter: true, _dirty_chunks_links.Count);
		Bench.bench("P calc_regions", "chunks");
		calc_regions();
		Bench.benchEnd("P calc_regions", "chunks", pSaveCounter: true, _dirty_chunks_regions.Count);
		Bench.bench("shuffle_region_tiles", "chunks");
		calc_shuffleRegionTiles();
		Bench.benchEnd("shuffle_region_tiles", "chunks", pSaveCounter: true, m_newRegions);
		Bench.bench("P calc_links", "chunks");
		calc_links();
		Bench.benchEnd("P calc_links", "chunks", pSaveCounter: true, _dirty_chunks_links.Count);
		Bench.bench("create_links", "chunks");
		calc_checkLinkResults();
		Bench.benchEnd("create_links", "chunks", pSaveCounter: true, _dirty_chunks_links.Count);
		Bench.bench("P calc_linked_regions", "chunks");
		calc_linkedRegions();
		Bench.benchEnd("P calc_linked_regions", "chunks", pSaveCounter: true, _dirty_chunks_links.Count);
		using ListPool<TileIsland> tNewIslands = new ListPool<TileIsland>();
		World.world.islands_calculator.findIslands(tNewIslands);
		Bench.bench("tile_corners_prepare", "chunks");
		calc_tileCornersPrepare(_region_set);
		m_dirtyCorners = _region_set.Count;
		Bench.benchEnd("tile_corners_prepare", "chunks", pSaveCounter: true, _dirty_chunks_links.Count);
		Bench.bench("center_regions", "chunks");
		calc_centerRegions();
		Bench.benchEnd("center_regions", "chunks", pSaveCounter: true, _region_set.Count);
		Bench.bench("island_region_edges", "chunks");
		int tCountIslandCorners = calc_islandRegionEdges(tNewIslands);
		Bench.benchEnd("island_region_edges", "chunks", pSaveCounter: true, tCountIslandCorners);
		Bench.bench("PH tile_edges", "chunks");
		calc_tileEdges();
		Bench.benchEnd("PH tile_edges", "chunks", pSaveCounter: true, _region_set.Count);
		Bench.bench("prepare_d_neighbour_islands", "chunks");
		prepareDirtyNeighbourIslands();
		Bench.benchEnd("prepare_d_neighbour_islands", "chunks", pSaveCounter: true, _temp_dirty_neighbours_islands.Count);
		Bench.bench("P neighbour_islands", "chunks");
		calc_neighbourIslands();
		Bench.benchEnd("P neighbour_islands", "chunks", pSaveCounter: true, _temp_dirty_neighbours_islands.Count);
		Bench.bench("clear_end", "chunks");
		_region_set.Clear();
		_dirty_chunks_links.Clear();
		_dirty_chunks_regions.Clear();
		World.world.city_zone_helper.city_place_finder.setDirty();
		World.world.region_path_finder.clearCache();
		Bench.benchEnd("clear_end", "chunks", pSaveCounter: false, 0L);
		Bench.benchSetValue("m_dirtyChunks", m_dirtyChunks, "chunks");
		Bench.benchSetValue("m_newRegions", m_newRegions, "chunks");
		Bench.benchSetValue("m_newLinks", m_newLinks, "chunks");
		Bench.benchSetValue("m_newIslands", m_newIslands, "chunks");
		Bench.benchSetValue("m_dirtyIslands", m_dirtyIslands, "chunks");
		Bench.benchSetValue("m_dirtyCorners", m_dirtyCorners, "chunks");
		Bench.benchSetValue("m_dirtyIslandNeighb", _temp_dirty_neighbours_islands.Count, "chunks");
		Bench.benchEnd("chunks", "chunks_total", pSaveCounter: false, 0L);
	}

	private void calc_clearRegions()
	{
		List<MapChunk> tDirtyRegionsList = _dirty_chunks_regions;
		int tCount1 = tDirtyRegionsList.Count;
		for (int i = 0; i < tCount1; i++)
		{
			tDirtyRegionsList[i].clearRegions();
		}
		List<MapChunk> tDirtyLinksList = _dirty_chunks_links;
		int tCount2 = tDirtyLinksList.Count;
		for (int j = 0; j < tCount2; j++)
		{
			tDirtyLinksList[j].clearIsland();
		}
	}

	private void calc_regions()
	{
		if (_is_parallel_enabled)
		{
			List<MapChunk> tDirtyChunks = _dirty_chunks_regions;
			int tCount = tDirtyChunks.Count;
			if (!_batches_enabled)
			{
				Parallel.For(0, tCount, World.world.parallel_options, delegate(int pIndex)
				{
					tDirtyChunks[pIndex].calculateRegions();
				});
				return;
			}
			int tDynamicBatchSize = ParallelHelper.getDynamicBatchSize(tCount);
			int tTotalBatches = ParallelHelper.calcTotalBatches(tCount, tDynamicBatchSize);
			Parallel.For(0, tTotalBatches, World.world.parallel_options, delegate(int pBatchIndex)
			{
				int num = ParallelHelper.calculateBatchBeg(pBatchIndex, tDynamicBatchSize);
				int num2 = ParallelHelper.calculateBatchEnd(num, tDynamicBatchSize, tCount);
				for (int j = num; j < num2; j++)
				{
					tDirtyChunks[j].calculateRegions();
				}
			});
		}
		else
		{
			List<MapChunk> tDirtyChunks2 = _dirty_chunks_regions;
			int tCount2 = tDirtyChunks2.Count;
			for (int i = 0; i < tCount2; i++)
			{
				tDirtyChunks2[i].calculateRegions();
			}
		}
	}

	private void calc_shuffleRegionTiles()
	{
		List<MapChunk> tDirtyChunks = _dirty_chunks_regions;
		int tCount = tDirtyChunks.Count;
		for (int i = 0; i < tCount; i++)
		{
			MapChunk tChunk = tDirtyChunks[i];
			m_newRegions += tChunk.regions.Count;
			tChunk.shuffleRegionTiles();
		}
	}

	private void calc_links()
	{
		if (_is_parallel_enabled)
		{
			List<MapChunk> tDirtyChunks = _dirty_chunks_links;
			int tCount = tDirtyChunks.Count;
			if (!_batches_enabled)
			{
				Parallel.For(0, tCount, World.world.parallel_options, delegate(int pIndex)
				{
					tDirtyChunks[pIndex].calculateLinks();
				});
				return;
			}
			int tDynamicBatchSize = ParallelHelper.getDynamicBatchSize(tCount);
			int tTotalBatches = ParallelHelper.calcTotalBatches(tCount, tDynamicBatchSize);
			Parallel.For(0, tTotalBatches, World.world.parallel_options, delegate(int pBatchIndex)
			{
				int num = ParallelHelper.calculateBatchBeg(pBatchIndex, tDynamicBatchSize);
				int num2 = ParallelHelper.calculateBatchEnd(num, tDynamicBatchSize, tCount);
				for (int j = num; j < num2; j++)
				{
					tDirtyChunks[j].calculateLinks();
				}
			});
		}
		else
		{
			List<MapChunk> tDirtyChunks2 = _dirty_chunks_links;
			int tCount2 = tDirtyChunks2.Count;
			for (int i = 0; i < tCount2; i++)
			{
				tDirtyChunks2[i].calculateLinks();
			}
		}
	}

	private void calc_checkLinkResults()
	{
		List<MapChunk> tDirtyChunks = _dirty_chunks_links;
		int tCount = tDirtyChunks.Count;
		for (int i = 0; i < tCount; i++)
		{
			tDirtyChunks[i].checkLinksResults();
		}
	}

	private void calc_linkedRegions()
	{
		if (_is_parallel_enabled)
		{
			List<MapChunk> tDirtyChunks = _dirty_chunks_links;
			int tCount = tDirtyChunks.Count;
			if (!_batches_enabled)
			{
				Parallel.For(0, tCount, World.world.parallel_options, delegate(int pIndex)
				{
					MapChunk pChunk = tDirtyChunks[pIndex];
					calculateRegionNeighbours(pChunk);
				});
				return;
			}
			int tDynamicBatchSize = ParallelHelper.getDynamicBatchSize(tCount);
			int tTotalBatches = ParallelHelper.calcTotalBatches(tCount, tDynamicBatchSize);
			Parallel.For(0, tTotalBatches, World.world.parallel_options, delegate(int pBatchIndex)
			{
				int num = ParallelHelper.calculateBatchBeg(pBatchIndex, tDynamicBatchSize);
				int num2 = ParallelHelper.calculateBatchEnd(num, tDynamicBatchSize, tCount);
				for (int j = num; j < num2; j++)
				{
					MapChunk pChunk = tDirtyChunks[j];
					calculateRegionNeighbours(pChunk);
				}
			});
		}
		else
		{
			List<MapChunk> tDirtyChunks2 = _dirty_chunks_links;
			int tCount2 = tDirtyChunks2.Count;
			for (int i = 0; i < tCount2; i++)
			{
				MapChunk tChunk = tDirtyChunks2[i];
				calculateRegionNeighbours(tChunk);
			}
		}
	}

	private void calc_tileCornersPrepare(HashSet<MapRegion> pSetRegionsResult)
	{
		List<MapChunk> tDirtyChunks = _dirty_chunks_links;
		int tCount = tDirtyChunks.Count;
		for (int tChunk = 0; tChunk < tCount; tChunk++)
		{
			List<MapRegion> tRegions = tDirtyChunks[tChunk].regions;
			int tCountRegions = tRegions.Count;
			for (int iRegion = 0; iRegion < tCountRegions; iRegion++)
			{
				MapRegion tReg = tRegions[iRegion];
				pSetRegionsResult.Add(tReg);
			}
		}
	}

	private void calc_centerRegions()
	{
		foreach (MapRegion item in _region_set)
		{
			item.calculateCenterRegion();
		}
	}

	private int calc_islandRegionEdges(ListPool<TileIsland> pNewIslands)
	{
		int tCountIslandCorners = 0;
		for (int iIsland = 0; iIsland < pNewIslands.Count; iIsland++)
		{
			TileIsland tIsland = pNewIslands[iIsland];
			List<MapRegion> tList = tIsland.regions.getSimpleList();
			for (int i = 0; i < tList.Count; i++)
			{
				MapRegion tRegion = tList[i];
				tCountIslandCorners++;
				if (!tRegion.center_region)
				{
					tIsland.insideRegionEdges.Add(tRegion);
				}
			}
		}
		return tCountIslandCorners;
	}

	private void calc_tileEdges()
	{
		HashSet<MapRegion> tRegionSet = _region_set;
		if (_is_parallel_enabled)
		{
			Parallel.ForEach(tRegionSet, World.world.parallel_options, delegate(MapRegion tReg)
			{
				tReg.calculateTileEdges();
			});
			return;
		}
		foreach (MapRegion item in tRegionSet)
		{
			item.calculateTileEdges();
		}
	}

	private void prepareDirtyNeighbourIslands()
	{
		ListPool<TileIsland> islands = World.world.islands_calculator.islands;
		List<TileIsland> tIslandsWithDirtyNeighbours = _temp_dirty_neighbours_islands;
		tIslandsWithDirtyNeighbours.Clear();
		foreach (ref TileIsland item in islands)
		{
			TileIsland tIsland = item;
			if (tIsland.isDirtyNeighbours())
			{
				tIslandsWithDirtyNeighbours.Add(tIsland);
			}
		}
	}

	private void calc_neighbourIslands()
	{
		List<TileIsland> tIslandsWithDirtyNeighbours = _temp_dirty_neighbours_islands;
		if (_is_parallel_enabled)
		{
			int tCount = tIslandsWithDirtyNeighbours.Count;
			if (!_batches_enabled)
			{
				Parallel.For(0, tCount, World.world.parallel_options, delegate(int pIndex)
				{
					tIslandsWithDirtyNeighbours[pIndex].calcNeighbourIslands();
				});
				return;
			}
			int tDynamicBatchSize = ParallelHelper.DEBUG_BATCH_SIZE;
			int tTotalBatches = ParallelHelper.calcTotalBatches(tCount, tDynamicBatchSize);
			Parallel.For(0, tTotalBatches, World.world.parallel_options, delegate(int pBatchIndex)
			{
				int num = ParallelHelper.calculateBatchBeg(pBatchIndex, tDynamicBatchSize);
				int num2 = ParallelHelper.calculateBatchEnd(num, tDynamicBatchSize, tCount);
				for (int j = num; j < num2; j++)
				{
					tIslandsWithDirtyNeighbours[j].calcNeighbourIslands();
				}
			});
		}
		else
		{
			for (int i = 0; i < tIslandsWithDirtyNeighbours.Count; i++)
			{
				tIslandsWithDirtyNeighbours[i].calcNeighbourIslands();
			}
		}
	}

	private void checkWrongIslands()
	{
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			MapChunk tChunk = tChunks[i];
			foreach (MapRegion tRegion in tChunk.regions)
			{
				foreach (WorldTile tTile in tRegion.tiles)
				{
					if (tTile.Type.layer_type != tRegion.island.type)
					{
						bool tWasDirtyChunkLastTime = _last_dirty_chunks.Contains(tChunk);
						bool tWasDirtyLinkLastTime = _last_dirty_links.Contains(tChunk);
						Debug.LogError("Wrong island type: " + tTile.Type.layer_type.ToString() + " != " + tRegion.island.type.ToString() + " " + tTile.chunk.id + " - was dirty: " + tWasDirtyChunkLastTime + " | " + tWasDirtyLinkLastTime);
						break;
					}
				}
			}
		}
	}

	private void calculateRegionNeighbours(MapChunk pChunk)
	{
		for (int i = 0; i < pChunk.regions.Count; i++)
		{
			pChunk.regions[i].calculateNeighbours();
		}
	}

	public int countRegions()
	{
		int tResult = 0;
		MapChunk[] tChunks = chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			MapChunk tChunk = tChunks[i];
			tResult += tChunk.regions.Count;
		}
		return tResult;
	}
}
