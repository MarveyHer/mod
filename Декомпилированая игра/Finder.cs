using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ai.behaviours;

public static class Finder
{
	private static readonly List<BaseSimObject> _list_objects = new List<BaseSimObject>(4096);

	private static MapChunk[] _chunks;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<Building> getBuildingsFromChunk(WorldTile pTile, int pChunkRadius, int pTileRadius = 0, bool pRandom = false)
	{
		int tX = pTile.chunk.x - pChunkRadius;
		int tY = pTile.chunk.y - pChunkRadius;
		int tWidth = pChunkRadius * 2 + 1;
		int tHeight = pChunkRadius * 2 + 1;
		int tCount = tWidth * tHeight;
		MapChunk[] tChunks = (_chunks = Toolbox.checkArraySize(_chunks, tCount));
		MapChunkManager tChunkManager = World.world.map_chunk_manager;
		int tTileRadius = pTileRadius * pTileRadius;
		int iChunk = 0;
		for (int iX = 0; iX < tWidth; iX++)
		{
			for (int iY = 0; iY < tHeight; iY++)
			{
				MapChunk tChunk = tChunkManager.get(tX + iX, tY + iY);
				if (tChunk == null)
				{
					tCount--;
				}
				else
				{
					tChunks[iChunk++] = tChunk;
				}
			}
		}
		if (pRandom)
		{
			foreach (MapChunk tChunk2 in tChunks.LoopRandom(tCount))
			{
				if (tChunk2 == null)
				{
					continue;
				}
				List<Building> tBuildings = tChunk2.objects.buildings_all;
				foreach (Building tBuilding in tBuildings.LoopRandom())
				{
					if (tBuilding.isAlive() && (tTileRadius == 0 || Toolbox.SquaredDistTile(tBuilding.current_tile, pTile) <= tTileRadius))
					{
						yield return tBuilding;
					}
				}
			}
			yield break;
		}
		foreach (MapChunk tChunk3 in tChunks.LoopRandom(tCount))
		{
			if (tChunk3 == null)
			{
				continue;
			}
			List<Building> tBuildings2 = tChunk3.objects.buildings_all;
			int i = 0;
			for (int tLen = tBuildings2.Count; i < tLen; i++)
			{
				Building tBuilding2 = tBuildings2[i];
				if (tBuilding2.isAlive() && (tTileRadius == 0 || Toolbox.SquaredDistTile(tBuilding2.current_tile, pTile) <= tTileRadius))
				{
					yield return tBuilding2;
				}
			}
		}
	}

	public static bool isEnemyNearOnSameIsland(Actor pActor, int pChunkRadius = 1)
	{
		foreach (Actor tActor in getUnitsFromChunk(pActor.current_tile, pChunkRadius))
		{
			if (pActor.isOnSameIsland(tActor) && tActor.kingdom.isEnemy(pActor.kingdom))
			{
				return true;
			}
		}
		return false;
	}

	public static bool isEnemyNearOnSameIslandAndCarnivore(Actor pActor, int pChunkRadius = 1)
	{
		foreach (Actor tActor in getUnitsFromChunk(pActor.current_tile, pChunkRadius))
		{
			if (pActor.isOnSameIsland(tActor))
			{
				if (tActor.isCarnivore())
				{
					return true;
				}
				if (tActor.kingdom.isEnemy(pActor.kingdom))
				{
					return true;
				}
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<Actor> getUnitsFromChunk(WorldTile pTile, int pChunkRadius, float pTileRadius = 0f, bool pRandom = false)
	{
		int tX = pTile.chunk.x - pChunkRadius;
		int tY = pTile.chunk.y - pChunkRadius;
		int tWidth = pChunkRadius * 2 + 1;
		int tHeight = pChunkRadius * 2 + 1;
		int tCount = tWidth * tHeight;
		MapChunk[] tChunks = (_chunks = Toolbox.checkArraySize(_chunks, tCount));
		MapChunkManager tChunkManager = World.world.map_chunk_manager;
		float tTileRadius = pTileRadius * pTileRadius;
		int iChunk = 0;
		for (int iX = 0; iX < tWidth; iX++)
		{
			for (int iY = 0; iY < tHeight; iY++)
			{
				MapChunk tChunk = tChunkManager.get(tX + iX, tY + iY);
				if (tChunk == null)
				{
					tCount--;
				}
				else
				{
					tChunks[iChunk++] = tChunk;
				}
			}
		}
		if (pRandom)
		{
			foreach (MapChunk tChunk2 in tChunks.LoopRandom(tCount))
			{
				if (tChunk2 == null)
				{
					continue;
				}
				List<Actor> tUnits = tChunk2.objects.units_all;
				foreach (Actor tActor in tUnits.LoopRandom())
				{
					if (tActor.isAlive() && (tTileRadius == 0f || !((float)Toolbox.SquaredDistTile(tActor.current_tile, pTile) > tTileRadius)))
					{
						yield return tActor;
					}
				}
			}
			yield break;
		}
		foreach (MapChunk tChunk3 in tChunks.LoopRandom(tCount))
		{
			if (tChunk3 == null)
			{
				continue;
			}
			List<Actor> tUnits2 = tChunk3.objects.units_all;
			int i = 0;
			for (int tLen = tUnits2.Count; i < tLen; i++)
			{
				Actor tActor2 = tUnits2[i];
				if (tActor2.isAlive() && (tTileRadius == 0f || !((float)Toolbox.SquaredDistTile(tActor2.current_tile, pTile) > tTileRadius)))
				{
					yield return tActor2;
				}
			}
		}
	}

	public static List<BaseSimObject> getAllObjectsInChunks(WorldTile pTile, int pTileRadius = 3)
	{
		List<BaseSimObject> tListObjects = _list_objects;
		tListObjects.Clear();
		fillAllObjectsFromChunk(pTile.chunk, pTile, pTileRadius, tListObjects);
		MapChunk[] tChunks = pTile.chunk.neighbours;
		for (int i = 0; i < tChunks.Length; i++)
		{
			fillAllObjectsFromChunk(tChunks[i], pTile, pTileRadius, tListObjects);
		}
		return tListObjects;
	}

	private static void fillAllObjectsFromChunk(MapChunk pChunk, WorldTile pTile, int pTileRadius, List<BaseSimObject> pListObjects)
	{
		int tTileRadius = pTileRadius * pTileRadius;
		List<long> tKingdoms = pChunk.objects.kingdoms;
		for (int i = 0; i < tKingdoms.Count; i++)
		{
			long tKingdom = tKingdoms[i];
			List<Actor> tUnits = pChunk.objects.getUnits(tKingdom);
			for (int j = 0; j < tUnits.Count; j++)
			{
				BaseSimObject tObject = tUnits[j];
				if (tObject.isAlive() && (pTileRadius == 0 || Toolbox.SquaredDistTile(tObject.current_tile, pTile) <= tTileRadius))
				{
					pListObjects.Add(tObject);
				}
			}
			List<Building> tBuildings = pChunk.objects.getBuildings(tKingdom);
			for (int k = 0; k < tBuildings.Count; k++)
			{
				BaseSimObject tObject2 = tBuildings[k];
				if (tObject2.isAlive() && (pTileRadius == 0 || Toolbox.SquaredDistTile(tObject2.current_tile, pTile) <= tTileRadius))
				{
					pListObjects.Add(tObject2);
				}
			}
		}
	}

	internal static IEnumerable<Actor> findSpeciesAroundTileChunk(WorldTile pTile, string pUnitID)
	{
		foreach (Actor tActor in getUnitsFromChunk(pTile, 1))
		{
			if (!(tActor.a.asset.id != pUnitID))
			{
				yield return tActor;
			}
		}
	}

	public static Building getClosestBuildingFrom(Actor pActor, IReadOnlyCollection<Building> pBuildingList)
	{
		return getClosestBuildingFrom(pActor.current_tile, pBuildingList);
	}

	public static Building getClosestBuildingFrom(WorldTile pTile, IReadOnlyCollection<Building> pBuildingList)
	{
		Building tTarget = null;
		float tBestDist = float.MaxValue;
		foreach (Building tB in pBuildingList)
		{
			if (!tB.isRekt() && tB.current_tile.isSameIsland(pTile))
			{
				float tDist = Toolbox.SquaredDistTile(tB.current_tile, pTile);
				if (tDist < tBestDist)
				{
					tTarget = tB;
					tBestDist = tDist;
				}
			}
		}
		return tTarget;
	}

	public static void clear()
	{
		_list_objects.Clear();
	}

	public static WorldTile findTileInChunk(WorldTile pTile, TileFinderType pTileType)
	{
		var (tChunks, tLength) = Toolbox.getAllChunksFromTile(pTile);
		foreach (MapChunk item in tChunks.LoopRandom(tLength))
		{
			foreach (MapRegion item2 in item.regions.LoopRandom())
			{
				foreach (WorldTile tRegionTile in item2.tiles.LoopRandom())
				{
					switch (pTileType)
					{
					case TileFinderType.FreeTile:
						if (!tRegionTile.isSameIsland(pTile) || tRegionTile.hasBuilding() || !tRegionTile.Type.ground)
						{
							continue;
						}
						break;
					case TileFinderType.Sand:
						if (!tRegionTile.Type.sand)
						{
							continue;
						}
						break;
					case TileFinderType.Water:
						if (tRegionTile.isTargeted() || !tRegionTile.Type.ocean)
						{
							continue;
						}
						break;
					case TileFinderType.Grass:
						if (!tRegionTile.isSameIsland(pTile) || tRegionTile.isTargeted() || !tRegionTile.Type.grass || tRegionTile.hasBuilding())
						{
							continue;
						}
						break;
					case TileFinderType.Dirt:
						if (!tRegionTile.isSameIsland(pTile) || tRegionTile.isTargeted() || !tRegionTile.Type.can_be_farm || tRegionTile.hasBuilding())
						{
							continue;
						}
						break;
					case TileFinderType.Biome:
						if (!tRegionTile.isSameIsland(pTile) || tRegionTile.isTargeted() || !tRegionTile.Type.is_biome || tRegionTile.hasBuilding())
						{
							continue;
						}
						break;
					default:
						if (!tRegionTile.isSameIsland(pTile))
						{
							continue;
						}
						break;
					}
					return tRegionTile;
				}
			}
		}
		return null;
	}
}
