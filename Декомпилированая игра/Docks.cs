using System.Collections.Generic;
using UnityPools;

public class Docks : BaseBuildingComponent
{
	public ListPool<WorldTile> tiles_ocean;

	private Dictionary<string, int> _boat_types;

	internal override void create(Building pBuilding)
	{
		base.create(pBuilding);
		tiles_ocean = new ListPool<WorldTile>();
		_boat_types = UnsafeCollectionPool<Dictionary<string, int>, KeyValuePair<string, int>>.Get();
	}

	public TileIsland getIsland()
	{
		if (building.hasCity())
		{
			return building.city.getTile()?.region.island;
		}
		return null;
	}

	public WorldTile getOceanTileInSameOcean(WorldTile pTile)
	{
		foreach (WorldTile tOceanTile in tiles_ocean.LoopRandom())
		{
			if (tOceanTile.isSameIsland(pTile))
			{
				return tOceanTile;
			}
		}
		return null;
	}

	public bool hasOceanTiles()
	{
		recalculateOceanTiles();
		return tiles_ocean.Count > 0;
	}

	public void recalculateOceanTiles()
	{
		tiles_ocean.Clear();
		WorldTile tTile = World.world.GetTile(building.current_tile.x - 4, building.current_tile.y);
		if (tTile != null && tTile.isGoodForBoat())
		{
			tiles_ocean.Add(tTile);
		}
		tTile = World.world.GetTile(building.current_tile.x + 5, building.current_tile.y);
		if (tTile != null && tTile.isGoodForBoat())
		{
			tiles_ocean.Add(tTile);
		}
		tTile = World.world.GetTile(building.current_tile.x, building.current_tile.y - 4);
		if (tTile != null && tTile.isGoodForBoat())
		{
			tiles_ocean.Add(tTile);
		}
		tTile = World.world.GetTile(building.current_tile.x, building.current_tile.y + 7);
		if (tTile != null && tTile.isGoodForBoat())
		{
			tiles_ocean.Add(tTile);
		}
		if (tiles_ocean.Count == 0)
		{
			building.startDestroyBuilding();
		}
	}

	public bool isDockGood()
	{
		if (tiles_ocean.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < tiles_ocean.Count; i++)
		{
			if (!tiles_ocean[i].Type.ocean)
			{
				return false;
			}
		}
		return true;
	}

	private bool ifStayingOnGround()
	{
		for (int i = 0; i < building.tiles.Count; i++)
		{
			if (building.tiles[i].Type.ground)
			{
				return true;
			}
		}
		return false;
	}

	public override void update(float pElapsed)
	{
		base.update(pElapsed);
		if (ifStayingOnGround())
		{
			building.getHit(1000f);
		}
		if (building.hasCity() && building.city.buildings.Count == 1)
		{
			building.getHit(1000f);
		}
	}

	public int countBoatTypes(string pType)
	{
		_boat_types.TryGetValue(pType, out var tValue);
		return tValue;
	}

	public bool isFull(string pType)
	{
		if (countBoatTypes(pType) >= 1)
		{
			return true;
		}
		return false;
	}

	public bool isOverfilled(string pType)
	{
		if (countBoatTypes(pType) > 1)
		{
			return true;
		}
		return false;
	}

	public Actor buildBoatFromHere(City pCity)
	{
		ActorAsset tNewBoatAsset = building.asset.getRandomBoatAssetToBuild(pCity);
		if (tNewBoatAsset == null)
		{
			return null;
		}
		if (countBoatTypes(tNewBoatAsset.boat_type) >= 1)
		{
			return null;
		}
		if (!pCity.hasEnoughResourcesFor(tNewBoatAsset.cost))
		{
			return null;
		}
		if (tiles_ocean.Count == 0)
		{
			recalculateOceanTiles();
			return null;
		}
		WorldTile tTile = tiles_ocean.GetRandom();
		if (!tTile.region.island.goodForDocks())
		{
			return null;
		}
		Actor tNewBoat = World.world.units.createNewUnit(tNewBoatAsset.id, tTile);
		if (tNewBoat == null)
		{
			return null;
		}
		addBoatToDock(tNewBoat);
		pCity.spendResourcesForBuildingAsset(tNewBoatAsset.cost);
		return tNewBoat;
	}

	public void clearBoatCounter()
	{
		_boat_types.Clear();
	}

	public void increaseBoatCounter(Actor pActor)
	{
		int tCount = countBoatTypes(pActor.asset.boat_type);
		tCount = (_boat_types[pActor.asset.boat_type] = tCount + 1);
	}

	public void addBoatToDock(Actor pBoat)
	{
		pBoat.setHomeBuilding(building);
		pBoat.joinCity(building.city);
		increaseBoatCounter(pBoat);
	}

	public override void Dispose()
	{
		base.Dispose();
		tiles_ocean.Dispose();
		tiles_ocean = null;
		_boat_types.Clear();
		UnsafeCollectionPool<Dictionary<string, int>, KeyValuePair<string, int>>.Release(_boat_types);
		_boat_types = null;
	}
}
