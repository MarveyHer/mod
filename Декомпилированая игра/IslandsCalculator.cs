using System.Collections.Generic;

public class IslandsCalculator
{
	private float _timer_update_actors;

	public ListPool<TileIsland> islands = new ListPool<TileIsland>();

	public readonly List<TileIsland> islands_ground = new List<TileIsland>();

	private readonly List<MapRegion> _temp_regions = new List<MapRegion>();

	private readonly List<MapRegion> _temp_regions_cur_wave = new List<MapRegion>();

	private readonly List<MapRegion> _temp_regions_next_wave = new List<MapRegion>();

	private int _last_island_id;

	private readonly HashSet<TileIsland> _dirty_islands = new HashSet<TileIsland>();

	private readonly Queue<MapRegion> _wave = new Queue<MapRegion>();

	private readonly Stack<TileIsland> _island_pool = new Stack<TileIsland>();

	public void prepareCalc()
	{
		_dirty_islands.Clear();
	}

	public void makeDirty(TileIsland pIsland)
	{
		_dirty_islands.Add(pIsland);
	}

	public void clearDirty()
	{
		using ListPool<TileIsland> tCurIslands = islands;
		islands = new ListPool<TileIsland>(tCurIslands.Count);
		foreach (TileIsland dirty_island in _dirty_islands)
		{
			dirty_island.clearRegionsFromIsland();
			dirty_island.insideRegionEdges.Clear();
			foreach (TileIsland connectedIsland in dirty_island.getConnectedIslands())
			{
				connectedIsland.setDirtyIslandNeighbours();
				MapChunkManager.m_dirtyIslands++;
			}
		}
		for (int i = 0; i < tCurIslands.Count; i++)
		{
			TileIsland tIsland = tCurIslands[i];
			if (tIsland.removed)
			{
				tIsland.reset();
				_island_pool.Push(tIsland);
			}
			else
			{
				islands.Add(tIsland);
			}
		}
	}

	public void clear()
	{
		_last_island_id = 0;
		ListPool<TileIsland> tIslands = islands;
		for (int i = 0; i < tIslands.Count; i++)
		{
			TileIsland tIsland = tIslands[i];
			tIsland.reset();
			_island_pool.Push(tIsland);
		}
		_dirty_islands.Clear();
		islands.Clear();
		islands_ground.Clear();
		_wave.Clear();
		_temp_regions.Clear();
		_temp_regions_cur_wave.Clear();
		_temp_regions_next_wave.Clear();
	}

	public WorldTile tryGetRandomGround()
	{
		WorldTile tTile = null;
		if (islands.Count > 0)
		{
			TileIsland tIsland = getRandomIslandGround();
			if (tIsland != null && tIsland.regions.Count > 0)
			{
				tTile = tIsland.getRandomTile();
			}
		}
		if (tTile == null)
		{
			tTile = World.world.tiles_list.GetRandom();
		}
		return tTile;
	}

	internal bool hasGround()
	{
		return islands_ground.Count > 0;
	}

	internal bool hasNonGround()
	{
		return islands.Count > islands_ground.Count;
	}

	internal float groundIslandRatio()
	{
		if (islands.Count == 0)
		{
			return 0f;
		}
		return (float)islands_ground.Count / (float)islands.Count;
	}

	internal float realGroundRatio()
	{
		if (!hasNonGround())
		{
			return 1f;
		}
		if (!hasGround())
		{
			return 0f;
		}
		int tGroundTiles = 0;
		int tNonGroundTiles = 0;
		foreach (ref TileIsland island in islands)
		{
			TileIsland tIsland = island;
			if (tIsland.type == TileLayerType.Ground)
			{
				tGroundTiles += tIsland.getTileCount();
			}
			else
			{
				tNonGroundTiles += tIsland.getTileCount();
			}
		}
		if (tGroundTiles == 0)
		{
			return 0f;
		}
		if (tNonGroundTiles == 0)
		{
			return 1f;
		}
		return (float)tGroundTiles / (float)(tGroundTiles + tNonGroundTiles);
	}

	internal TileIsland getRandomIslandGroundWeighted(bool pMinRegions = true)
	{
		if (islands_ground.Count == 0)
		{
			return null;
		}
		int tRegions = 0;
		for (int i = 0; i < islands_ground.Count; i++)
		{
			TileIsland tIsland = islands_ground[i];
			if (!pMinRegions || tIsland.regions.Count >= 4)
			{
				tRegions += tIsland.regions.Count;
			}
		}
		if (tRegions == 0)
		{
			return null;
		}
		using ListPool<TileIsland> tTileIslands = new ListPool<TileIsland>(tRegions);
		for (int j = 0; j < islands_ground.Count; j++)
		{
			TileIsland tIsland2 = islands_ground[j];
			if (!pMinRegions || tIsland2.regions.Count >= 4)
			{
				tTileIslands.AddTimes(tIsland2.regions.Count, tIsland2);
			}
		}
		return tTileIslands.GetRandom();
	}

	internal TileIsland getRandomIslandGround(bool pMinRegions = true)
	{
		if (islands_ground.Count == 0)
		{
			return null;
		}
		if (!pMinRegions)
		{
			return islands_ground.GetRandom();
		}
		foreach (TileIsland tIsland in islands_ground.LoopRandom())
		{
			if (tIsland.regions.Count >= 4)
			{
				return tIsland;
			}
		}
		return null;
	}

	internal TileIsland getRandomIslandNonGroundWeighted(bool pMinRegions = true)
	{
		if (islands.Count == 0)
		{
			return null;
		}
		if (islands_ground.Count == islands.Count)
		{
			return null;
		}
		int tRegions = 0;
		for (int i = 0; i < islands.Count; i++)
		{
			TileIsland tIsland = islands[i];
			if (tIsland.type != TileLayerType.Ground && (!pMinRegions || tIsland.regions.Count >= 4))
			{
				tRegions += tIsland.regions.Count;
			}
		}
		if (tRegions == 0)
		{
			return null;
		}
		using ListPool<TileIsland> tTileIslands = new ListPool<TileIsland>(tRegions);
		for (int j = 0; j < islands.Count; j++)
		{
			TileIsland tIsland2 = islands[j];
			if (tIsland2.type != TileLayerType.Ground && (!pMinRegions || tIsland2.regions.Count >= 4))
			{
				tTileIslands.AddTimes(tIsland2.regions.Count, tIsland2);
			}
		}
		return tTileIslands.GetRandom();
	}

	internal TileIsland getRandomIslandNonGround(bool pMinRegions = true)
	{
		if (islands.Count == 0)
		{
			return null;
		}
		if (islands_ground.Count == islands.Count)
		{
			return null;
		}
		foreach (TileIsland tIsland in islands.LoopRandom())
		{
			if (tIsland.type != TileLayerType.Ground && (!pMinRegions || tIsland.regions.Count >= 4))
			{
				return tIsland;
			}
		}
		return null;
	}

	public int countLandIslands()
	{
		int tResult = 0;
		for (int i = 0; i < islands.Count; i++)
		{
			TileIsland tIsland = islands[i];
			if (tIsland.type == TileLayerType.Ground && tIsland.regions.Count >= 4)
			{
				tResult++;
			}
		}
		return tResult;
	}

	internal void recalcActors()
	{
		ListPool<TileIsland> tIslands = islands;
		for (int i = 0; i < tIslands.Count; i++)
		{
			tIslands[i].actors.Clear();
		}
		List<Actor> tList = World.world.units.getSimpleList();
		for (int j = 0; j < tList.Count; j++)
		{
			Actor tActor = tList[j];
			if (tActor.isAlive())
			{
				tActor.current_tile.region.island.actors.Add(tActor);
			}
		}
	}

	private void clearCaches()
	{
		for (int i = 0; i < islands.Count; i++)
		{
			islands[i].clearCache();
		}
	}

	public void findIslands(ListPool<TileIsland> pNewIslands)
	{
		Bench.bench("find_islands_prepare", "chunks");
		_temp_regions.Clear();
		islands_ground.Clear();
		clearCaches();
		Bench.benchEnd("find_islands_prepare", "chunks", pSaveCounter: false, 0L);
		Bench.bench("find_islands_temp_regions", "chunks");
		MapChunk[] tChunks = World.world.map_chunk_manager.chunks;
		int tLen = tChunks.Length;
		for (int i = 0; i < tLen; i++)
		{
			MapChunk tChunk = tChunks[i];
			for (int j = 0; j < tChunk.regions.Count; j++)
			{
				MapRegion tRegion = tChunk.regions[j];
				if (tRegion.island == null)
				{
					tRegion.is_island_checked = false;
					_temp_regions.Add(tRegion);
				}
			}
		}
		Bench.benchEnd("find_islands_temp_regions", "chunks", pSaveCounter: true, _temp_regions.Count);
		Bench.bench("find_islands_new_islands", "chunks");
		for (int k = 0; k < _temp_regions.Count; k++)
		{
			MapRegion tRegion2 = _temp_regions[k];
			if (tRegion2.island == null)
			{
				TileIsland tIsland = newIslandFrom(tRegion2);
				pNewIslands.Add(tIsland);
				MapChunkManager.m_newIslands++;
			}
		}
		Bench.benchEnd("find_islands_new_islands", "chunks", pSaveCounter: true, MapChunkManager.m_newIslands);
		Bench.bench("find_islands_fin", "chunks");
		for (int l = 0; l < islands.Count; l++)
		{
			TileIsland tIsland2 = islands[l];
			tIsland2.countTiles();
			if (tIsland2.type == TileLayerType.Ground)
			{
				islands_ground.Add(tIsland2);
			}
		}
		Bench.benchEnd("find_islands_fin", "chunks", pSaveCounter: false, 0L);
	}

	private TileIsland newIslandFrom(MapRegion pRegion)
	{
		_temp_regions_cur_wave.Clear();
		_temp_regions_next_wave.Clear();
		TileIsland tIsland;
		if (_island_pool.Count > 0)
		{
			tIsland = _island_pool.Pop();
		}
		else
		{
			tIsland = new TileIsland(_last_island_id);
			_last_island_id++;
		}
		tIsland.reset();
		tIsland.type = pRegion.type;
		islands.Add(tIsland);
		_temp_regions_next_wave.Add(pRegion);
		_wave.Clear();
		_wave.Enqueue(pRegion);
		startFill(tIsland);
		return tIsland;
	}

	private void startFill(TileIsland pIsland)
	{
		while (_wave.Count > 0)
		{
			MapRegion tCurRegion = _wave.Dequeue();
			if (!tCurRegion.is_island_checked)
			{
				tCurRegion.island = pIsland;
				pIsland.addRegion(tCurRegion);
			}
			tCurRegion.is_island_checked = true;
			for (int j = 0; j < tCurRegion.neighbours.Count; j++)
			{
				MapRegion tNeighbour = tCurRegion.neighbours[j];
				if (!tNeighbour.is_island_checked)
				{
					tNeighbour.is_island_checked = true;
					tNeighbour.island = pIsland;
					pIsland.addRegion(tNeighbour);
					_wave.Enqueue(tNeighbour);
				}
			}
		}
		pIsland.regions.checkAddRemove();
	}
}
