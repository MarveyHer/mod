using System;
using System.Collections.Generic;
using UnityEngine;

public class MapChunk : IDisposable
{
	private readonly Queue<WorldTile> _wave = new Queue<WorldTile>(256);

	public readonly ChunkObjectContainer objects = new ChunkObjectContainer();

	public MapChunk[] neighbours;

	public MapChunk[] neighbours_all;

	public readonly WorldTile[] tiles = new WorldTile[256];

	public readonly List<MapRegion> regions = new List<MapRegion>(4);

	public List<WorldTile> edges_all;

	public List<WorldTile> chunk_bounds;

	public readonly List<WorldTile> bounds_left = new List<WorldTile>(16);

	public readonly List<WorldTile> bounds_up = new List<WorldTile>(16);

	public readonly List<WorldTile> bounds_down = new List<WorldTile>(16);

	public readonly List<WorldTile> bounds_right = new List<WorldTile>(16);

	public float world_center_x;

	public float world_center_y;

	public WorldTile edge_up_left;

	public WorldTile edge_up_right;

	public WorldTile edge_down_left;

	public WorldTile edge_down_right;

	private WorldTile _edge_up_left_connection;

	private WorldTile _edge_up_right_connection;

	private WorldTile _edge_down_left_connection;

	private WorldTile _edge_down_right_connection;

	public bool world_edge;

	public int x;

	public int y;

	public int id;

	public Color color;

	public bool dirty_regions;

	public bool dirty_links;

	private readonly List<WorldTile> _temp_tiles = new List<WorldTile>(16);

	private readonly List<TempLinkStruct> _new_hashes = new List<TempLinkStruct>();

	internal MapChunk chunk_up;

	internal MapChunk chunk_down;

	internal MapChunk chunk_left;

	internal MapChunk chunk_right;

	private readonly StackPool<MapRegion> _region_pool = new StackPool<MapRegion>();

	public readonly List<TileZone> zones = new List<TileZone>();

	private bool _buildings_dirty;

	private bool _tile_types_dirty;

	private const int MAX_TILE_TYPES = 256;

	private readonly Dictionary<TileTypeBase, int> _tile_types_count = new Dictionary<TileTypeBase, int>(256);

	private readonly List<MusicBoxTileData> _simple_data = new List<MusicBoxTileData>();

	public bool buildings_dirty => _buildings_dirty;

	internal void addTile(WorldTile pTile, int pX, int pY)
	{
		tiles[tiles.FreeIndex()] = pTile;
	}

	internal void addNeighbour(MapChunk pNeighbour, TileDirection pDirection, IList<MapChunk> pNeighbours, IList<MapChunk> pNeighboursAll, bool pDiagonal = false)
	{
		if (pNeighbour == null)
		{
			world_edge = true;
			return;
		}
		if (!pDiagonal)
		{
			pNeighbours.Add(pNeighbour);
		}
		pNeighboursAll.Add(pNeighbour);
		switch (pDirection)
		{
		case TileDirection.Up:
			chunk_up = pNeighbour;
			break;
		case TileDirection.Down:
			chunk_down = pNeighbour;
			break;
		case TileDirection.Left:
			chunk_left = pNeighbour;
			break;
		case TileDirection.Right:
			chunk_right = pNeighbour;
			break;
		}
	}

	public void calculateRegions()
	{
		dirty_regions = false;
		WorldTile[] tTiles = tiles;
		List<MapRegion> tRegions = regions;
		StackPool<MapRegion> tRegionPool = _region_pool;
		clearTiles();
		int tCount = tTiles.Length;
		for (int i = 0; i < tCount; i++)
		{
			WorldTile tTile = tTiles[i];
			if (tTile.region == null)
			{
				MapRegion tRegion = tRegionPool.get();
				tRegion.reset();
				tRegion.type = tTile.Type.layer_type;
				tRegion.chunk = this;
				fillRegion(tTile, tRegion);
				tRegion.id = tTile.zone.id * 1000 + tRegions.Count;
				tRegions.Add(tRegion);
				if (tRegion.tiles.Count == tTiles.Length)
				{
					break;
				}
			}
		}
		clearTiles(pClearRegion: false);
		tCount = tRegions.Count;
		for (int j = 0; j < tCount; j++)
		{
			tRegions[j].checkZones();
		}
	}

	private void clearTiles(bool pClearRegion = true)
	{
		WorldTile[] tTiles = tiles;
		if (pClearRegion)
		{
			int tCount = tTiles.Length;
			for (int i = 0; i < tCount; i++)
			{
				WorldTile obj = tTiles[i];
				obj.is_checked_tile = false;
				obj.region = null;
			}
		}
		else
		{
			int tCount2 = tTiles.Length;
			for (int j = 0; j < tCount2; j++)
			{
				tTiles[j].is_checked_tile = false;
			}
		}
	}

	internal void shuffleRegionTiles()
	{
		for (int i = 0; i < regions.Count; i++)
		{
			MapRegion tRegion = regions[i];
			if (regions.Count > 1)
			{
				tRegion.center_region = false;
			}
			else
			{
				tRegion.center_region = true;
			}
			tRegion.tiles.Shuffle();
		}
	}

	private void fillRegion(WorldTile pTile, MapRegion pTargetRegion)
	{
		Queue<WorldTile> tWave = _wave;
		tWave.Enqueue(pTile);
		while (tWave.Count > 0)
		{
			WorldTile tTile = tWave.Dequeue();
			tTile.is_checked_tile = true;
			tTile.region = pTargetRegion;
			pTargetRegion.tiles.Add(tTile);
			processTileNeighbours(tTile, pTargetRegion, tWave);
		}
		tWave.Clear();
	}

	private void processTileNeighbours(WorldTile pTileMain, MapRegion pTargetRegion, Queue<WorldTile> pWave)
	{
		WorldTile[] tNeighbours = pTileMain.neighboursAll;
		foreach (WorldTile tTileNeighbour in tNeighbours)
		{
			TileTypeBase tTypeNeighbour = tTileNeighbour.Type;
			if (!tTileNeighbour.is_checked_tile)
			{
				if (tTypeNeighbour.layer_type != pTileMain.region.type || tTileNeighbour.chunk != this)
				{
					pTargetRegion.edge_tiles_set.Add(tTileNeighbour);
				}
				else if (!isDiagonalBlockedByCorners(pTileMain, tTileNeighbour))
				{
					tTileNeighbour.is_checked_tile = true;
					tTileNeighbour.region = pTargetRegion;
					pWave.Enqueue(tTileNeighbour);
				}
			}
		}
	}

	private bool isDiagonalBlockedByCorners(WorldTile pTileFrom, WorldTile pTileTo)
	{
		int tDx = pTileTo.x - pTileFrom.x;
		int tDy = pTileTo.y - pTileFrom.y;
		if (Math.Abs(tDx) != 1 || Math.Abs(tDy) != 1)
		{
			return false;
		}
		WorldTile tTileX = World.world.GetTile(pTileFrom.x + tDx, pTileFrom.y);
		WorldTile tTileY = World.world.GetTile(pTileFrom.x, pTileFrom.y + tDy);
		bool num = tTileX?.Type.block ?? true;
		bool yBlocked = tTileY?.Type.block ?? true;
		return num || yBlocked;
	}

	public void clearObjects(bool pForceClearBuildings = false)
	{
		if (pForceClearBuildings)
		{
			setBuildingsDirty();
		}
		objects.reset(buildings_dirty);
		_temp_tiles.Clear();
		_new_hashes.Clear();
	}

	public void clearRegions()
	{
		clearIsland();
		for (int i = 0; i < regions.Count; i++)
		{
			MapRegion tRegion = regions[i];
			tRegion.reset();
			_region_pool.release(tRegion);
		}
		regions.Clear();
	}

	public void Dispose()
	{
		clearRegions();
		clearObjects(pForceClearBuildings: true);
		setBuildingsDirty();
		objects.Dispose();
		neighbours.Clear();
		neighbours_all.Clear();
		neighbours = null;
		neighbours_all = null;
		tiles.Clear();
		_wave.Clear();
		edges_all?.Clear();
		chunk_bounds?.Clear();
		bounds_left.Clear();
		bounds_up.Clear();
		bounds_down.Clear();
		bounds_right.Clear();
		chunk_up = null;
		chunk_down = null;
		chunk_left = null;
		chunk_right = null;
		edge_down_left = null;
		edge_down_right = null;
		edge_up_left = null;
		edge_up_right = null;
		_edge_down_left_connection = null;
		_edge_down_right_connection = null;
		_edge_up_left_connection = null;
		_edge_up_right_connection = null;
		_region_pool.clear();
		zones.Clear();
	}

	public void clearIsland()
	{
		for (int i = 0; i < regions.Count; i++)
		{
			MapRegion tRegion = regions[i];
			tRegion.clear();
			if (tRegion.island != null)
			{
				World.world.islands_calculator.makeDirty(tRegion.island);
			}
		}
	}

	internal void combineEdges()
	{
		HashSet<WorldTile> tSet = new HashSet<WorldTile>();
		edges_all = new List<WorldTile>(tSet);
		tSet.Clear();
		tSet.UnionWith(bounds_down);
		tSet.UnionWith(bounds_left);
		tSet.UnionWith(bounds_right);
		tSet.UnionWith(bounds_up);
		chunk_bounds = new List<WorldTile>(tSet);
	}

	public void generateEdgeConnections()
	{
		_edge_up_left_connection = chunk_left?.chunk_up?.edge_down_right;
		_edge_up_right_connection = chunk_right?.chunk_up?.edge_down_left;
		_edge_down_left_connection = chunk_left?.chunk_down?.edge_up_right;
		_edge_down_right_connection = chunk_right?.chunk_down?.edge_up_left;
	}

	public void checkLinksResults()
	{
		for (int i = 0; i < _new_hashes.Count; i++)
		{
			TempLinkStruct tStruct = _new_hashes[i];
			RegionLinkHashes.addHash(tStruct.hash, tStruct.region);
		}
		MapChunkManager.m_newLinks += _new_hashes.Count;
		_new_hashes.Clear();
	}

	internal void calculateLinks()
	{
		dirty_links = false;
		calculateLink(bounds_right, chunk_right?.bounds_left, LinkDirection.Right, LinkDirection.LR, pUseTargetList: true);
		calculateLink(bounds_left, chunk_left?.bounds_right, LinkDirection.Left, LinkDirection.LR);
		calculateLink(bounds_up, chunk_up?.bounds_down, LinkDirection.Up, LinkDirection.UD, pUseTargetList: true);
		calculateLink(bounds_down, chunk_down?.bounds_up, LinkDirection.Down, LinkDirection.UD);
		checkSpecialDiagonalConnection(edge_up_left, _edge_up_left_connection, LinkDirection.Up, LinkDirection.UD);
		checkSpecialDiagonalConnection(edge_up_right, _edge_up_right_connection, LinkDirection.Up, LinkDirection.UD, pUseTargetList: true);
		checkSpecialDiagonalConnection(edge_down_left, _edge_down_left_connection, LinkDirection.Down, LinkDirection.UD);
		checkSpecialDiagonalConnection(edge_down_right, _edge_down_right_connection, LinkDirection.Down, LinkDirection.UD, pUseTargetList: true);
	}

	private void checkSpecialDiagonalConnection(WorldTile pTileMain, WorldTile pTileTarget, LinkDirection pDirection, LinkDirection pGroupID, bool pUseTargetList = false)
	{
		if (pTileTarget != null && WorldTile.isSameLayer(pTileMain, pTileTarget) && !isDiagonalBlockedByCorners(pTileMain, pTileTarget))
		{
			if (pUseTargetList)
			{
				_temp_tiles.Add(pTileTarget);
			}
			else
			{
				_temp_tiles.Add(pTileMain);
			}
			makeLink(_temp_tiles, pDirection, pGroupID, pTileMain.region);
		}
	}

	private void calculateLink(List<WorldTile> pOurBounds, List<WorldTile> pTargetEdgeTiles, LinkDirection pDirection, LinkDirection pGroupID, bool pUseTargetList = false)
	{
		if (pTargetEdgeTiles == null)
		{
			return;
		}
		int tCount = pOurBounds.Count;
		List<WorldTile> tNewConnectionTiles = _temp_tiles;
		tNewConnectionTiles.Clear();
		bool tStarted = false;
		MapRegion tRegion = null;
		for (int i = 0; i < tCount; i++)
		{
			bool tIsLastElement = i == tCount - 1;
			WorldTile tOurTile = pOurBounds[i];
			WorldTile tTargetTile = pTargetEdgeTiles[i];
			bool tGoodToContinue = WorldTile.isSameLayer(tOurTile, tTargetTile);
			if (tGoodToContinue && !tStarted)
			{
				tStarted = true;
				tRegion = tOurTile.region;
			}
			if (!tIsLastElement)
			{
				if (tGoodToContinue)
				{
					WorldTile tNextOurTile = pOurBounds[i + 1];
					tGoodToContinue = WorldTile.isSameLayer(tOurTile, tNextOurTile);
				}
				if (tGoodToContinue)
				{
					WorldTile tNextTargetTile = pTargetEdgeTiles[i + 1];
					tGoodToContinue = WorldTile.isSameLayer(tOurTile, tNextTargetTile);
				}
			}
			if (!tGoodToContinue && !tStarted && tryDiagonal(tOurTile, tTargetTile, pDirection, pGroupID, pUseTargetList, tNewConnectionTiles))
			{
				tStarted = false;
				tRegion = null;
				continue;
			}
			if (tIsLastElement)
			{
				tGoodToContinue = false;
			}
			if (tStarted || tGoodToContinue)
			{
				if (!tStarted && tGoodToContinue)
				{
					tStarted = true;
					tRegion = tOurTile.region;
					saveToConnection(tNewConnectionTiles, tOurTile, tTargetTile, pUseTargetList);
				}
				else if (tStarted && !tGoodToContinue)
				{
					saveToConnection(tNewConnectionTiles, tOurTile, tTargetTile, pUseTargetList);
					makeLink(tNewConnectionTiles, pDirection, pGroupID, tRegion);
					tStarted = false;
					tRegion = null;
				}
				else
				{
					saveToConnection(tNewConnectionTiles, tOurTile, tTargetTile, pUseTargetList);
				}
			}
		}
	}

	private bool tryDiagonal(WorldTile pMainTile, WorldTile pTargetTile, LinkDirection pDirection, LinkDirection pGroupID, bool pUseTargetList, List<WorldTile> pListConnections)
	{
		bool tAnyDiagonalFound = false;
		WorldTile tDiagonalTile1 = getDiagonalConnection(pMainTile, pTargetTile, pDirection, pFirst: true);
		if (tDiagonalTile1 != null)
		{
			saveToConnection(pListConnections, pMainTile, tDiagonalTile1, pUseTargetList);
			makeLink(pListConnections, pDirection, pGroupID, pMainTile.region);
			tAnyDiagonalFound = true;
		}
		WorldTile tDiagonalTile2 = getDiagonalConnection(pMainTile, pTargetTile, pDirection, pFirst: false);
		if (tDiagonalTile2 != null)
		{
			saveToConnection(pListConnections, pMainTile, tDiagonalTile2, pUseTargetList);
			makeLink(pListConnections, pDirection, pGroupID, pMainTile.region);
			tAnyDiagonalFound = true;
		}
		return tAnyDiagonalFound;
	}

	private void saveToConnection(List<WorldTile> pList, WorldTile pOurTile, WorldTile pTargetTile, bool pUseTargetList)
	{
		if (pUseTargetList)
		{
			pList.Add(pTargetTile);
		}
		else
		{
			pList.Add(pOurTile);
		}
	}

	private WorldTile getDiagonalConnection(WorldTile pOurTile, WorldTile pTargetTile, LinkDirection pDirection, bool pFirst)
	{
		TileLayerType tMainType = pOurTile.Type.layer_type;
		WorldTile tResultTile = null;
		switch (pDirection)
		{
		case LinkDirection.Up:
		case LinkDirection.Down:
			tResultTile = (pFirst ? pTargetTile.tile_right : pTargetTile.tile_left);
			break;
		case LinkDirection.Left:
		case LinkDirection.Right:
			tResultTile = (pFirst ? pTargetTile.tile_up : pTargetTile.tile_down);
			break;
		}
		if (tResultTile == null)
		{
			return null;
		}
		if (isDiagonalBlockedByCorners(pOurTile, tResultTile))
		{
			return null;
		}
		if (tResultTile.Type.layer_type != tMainType)
		{
			return null;
		}
		return tResultTile;
	}

	private void makeLink(List<WorldTile> pConnectionList, LinkDirection pDirection, LinkDirection pGroupID, MapRegion pRegionMain)
	{
		int tLinkHashId = pRegionMain.newConnection(pConnectionList, pDirection, pGroupID);
		_new_hashes.Add(new TempLinkStruct
		{
			region = pRegionMain,
			hash = tLinkHashId
		});
		pConnectionList.Clear();
	}

	public void setBuildingsDirty()
	{
		_buildings_dirty = true;
	}

	public void finishBuildingsCheck()
	{
		_buildings_dirty = false;
	}

	public List<MusicBoxTileData> getSimpleData()
	{
		if (_tile_types_dirty)
		{
			musicBoxCheckCount();
		}
		return _simple_data;
	}

	private void musicBoxCheckCount()
	{
		_tile_types_dirty = false;
		_tile_types_count.Clear();
		_simple_data.Clear();
		int i = 0;
		TileTypeBase key;
		for (int tLen = zones.Count; i < tLen; i++)
		{
			foreach (KeyValuePair<TileTypeBase, HashSet<WorldTile>> tileType in zones[i].getTileTypes())
			{
				tileType.Deconstruct(out key, out var value);
				TileTypeBase tKey = key;
				HashSet<WorldTile> tSet = value;
				if (tKey.music_assets != null)
				{
					int tValue = tSet.Count;
					if (!_tile_types_count.TryAdd(tKey, tValue))
					{
						Dictionary<TileTypeBase, int> tile_types_count = _tile_types_count;
						key = tKey;
						tile_types_count[key] += tValue;
					}
				}
			}
		}
		foreach (KeyValuePair<TileTypeBase, int> item in _tile_types_count)
		{
			item.Deconstruct(out key, out var value2);
			TileTypeBase tKey2 = key;
			int tValue2 = value2;
			_simple_data.Add(new MusicBoxTileData
			{
				tile_type_id = tKey2.index_id,
				amount = tValue2
			});
		}
	}

	public Dictionary<TileTypeBase, int> getTileTypesCount()
	{
		if (_tile_types_dirty)
		{
			musicBoxCheckCount();
		}
		return _tile_types_count;
	}

	public int countTilesOfType(TileTypeBase pType)
	{
		if (_tile_types_dirty)
		{
			musicBoxCheckCount();
		}
		_tile_types_count.TryGetValue(pType, out var tResult);
		return tResult;
	}

	public void setTileTypesDirty()
	{
		_tile_types_dirty = true;
	}
}
